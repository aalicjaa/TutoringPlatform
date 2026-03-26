import { AppBar, Toolbar, Typography, Button, Stack, IconButton, Box } from "@mui/material";
import { Link as RouterLink, useNavigate } from "react-router-dom";
import LightModeIcon from "@mui/icons-material/LightMode";
import DarkModeIcon from "@mui/icons-material/DarkMode";
import LogoutIcon from "@mui/icons-material/Logout";
import { tokenStorage } from "../api/http";

type Props = {
  toggleTheme: () => void;
  themeMode: "light" | "dark";
  onLogout: () => void;
};

const ACCENT = "#f29f05";

export default function AppNavbar({ toggleTheme, themeMode, onLogout }: Props) {
  const navigate = useNavigate();

  const logout = () => {
    tokenStorage.clear();
    onLogout();
    navigate("/login");
  };

  return (
    <AppBar
      position="sticky"
      elevation={0}
      sx={{
        background: "linear-gradient(90deg, #1e3a5f 0%, #2b4f78 100%)",
        boxShadow: "0 2px 8px rgba(0,0,0,0.08)",
      }}
    >
      <Toolbar
        sx={{
          justifyContent: "space-between",
          maxWidth: 1200,
          width: "100%",
          mx: "auto",
        }}
      >
        <Typography
          variant="h6"
          component={RouterLink}
          to="/tutors"
          sx={{
            color: "inherit",
            textDecoration: "none",
            fontWeight: 800,
            letterSpacing: 0.3,
            display: "flex",
            alignItems: "center",
            gap: 1.2,
          }}
        >
          <Box
            sx={{
              width: 30,
              height: 30,
              borderRadius: "50%",
              display: "flex",
              alignItems: "center",
              justifyContent: "center",
              fontSize: 13,
              fontWeight: 900,
              background: `linear-gradient(135deg, ${ACCENT}, ${ACCENT})`,
              color: "#1e293b",
              boxShadow: "0 2px 6px rgba(0,0,0,0.15)",
            }}
          >
            EM
          </Box>

          <Box component="span">
            Edu<span style={{ color: ACCENT }}>Match</span>
          </Box>
        </Typography>

        <Stack direction="row" spacing={1} alignItems="center">
          <Button color="inherit" component={RouterLink} to="/tutors">
            Wszystkie oferty
          </Button>

          <Button color="inherit" component={RouterLink} to="/my-bookings">
            Moje rezerwacje
          </Button>

          <Button color="inherit" component={RouterLink} to="/tutor-panel">
            Panel tutora
          </Button>

          <IconButton color="inherit" onClick={toggleTheme}>
            {themeMode === "light" ? <DarkModeIcon /> : <LightModeIcon />}
          </IconButton>

          <IconButton color="inherit" onClick={logout}>
            <LogoutIcon />
          </IconButton>
        </Stack>
      </Toolbar>
    </AppBar>
  );
}
