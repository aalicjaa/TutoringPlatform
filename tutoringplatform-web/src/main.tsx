import { StrictMode, useMemo, useState, useEffect } from "react";
import { createRoot } from "react-dom/client";
import { BrowserRouter } from "react-router-dom";
import { CssBaseline, ThemeProvider, createTheme } from "@mui/material";
import App from "./App";
import { tokenStorage } from "./api/http";

function Root() {
  const [mode, setMode] = useState<"light" | "dark">(
    (localStorage.getItem("themeMode") as "light" | "dark") || "light"
  );

  useEffect(() => {
    const fresh = sessionStorage.getItem("fresh");
    if (!fresh) {
      tokenStorage.clear();
      sessionStorage.setItem("fresh", "1");
    }
  }, []);

  const toggleTheme = () => {
    const next = mode === "light" ? "dark" : "light";
    localStorage.setItem("themeMode", next);
    setMode(next);
  };

  const theme = useMemo(
    () =>
      createTheme({
        palette: {
          mode,
          primary: {
            main: "#1e3a5f",
            light: "#335c8a",
            dark: "#142a44",
          },
          secondary: {
            main: "#f29f05",
            light: "#ffd166",
            dark: "#d48806",
          },
          success: { main: "#2e7d32" },
          warning: { main: "#fb8c00" },
          background:
            mode === "light"
              ? { default: "#f6faf6", paper: "#ffffff" }
              : { default: "#0f1a12", paper: "#16241a" },
        },
        shape: { borderRadius: 14 },
      }),
    [mode]
  );

  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <BrowserRouter>
        <App toggleTheme={toggleTheme} themeMode={mode} />
      </BrowserRouter>
    </ThemeProvider>
  );
}

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <Root />
  </StrictMode>
);
