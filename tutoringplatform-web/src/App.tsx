import { Navigate, Route, Routes } from "react-router-dom";
import { Box } from "@mui/material";
import { useState } from "react";
import LoginPage from "./pages/LoginPage";
import RegisterPage from "./pages/RegisterPage";
import TutorsPage from "./pages/TutorsPage";
import TutorPanelPage from "./pages/TutorPanelPage";
import TutorDetailsPage from "./pages/TutorDetailsPage";
import MyBookingsPage from "./pages/MyBookingsPage";
import AppNavbar from "./components/AppNavbar";
import AppFooter from "./components/AppFooter";
import { tokenStorage } from "./api/http";
import PrivacyPage from "./pages/PrivacyPage";
import TermsPage from "./pages/TermsPage";
import ContactPage from "./pages/ContactPage";


type Props = {
  toggleTheme: () => void;
  themeMode: "light" | "dark";
};

export default function App({ toggleTheme, themeMode }: Props) {
  const [isAuthed, setIsAuthed] = useState(() => !!tokenStorage.get());

  return (
    <Box sx={{ height: "100vh", display: "flex", flexDirection: "column", overflow: "hidden" }}>
      {isAuthed && (
        <AppNavbar toggleTheme={toggleTheme} themeMode={themeMode} onLogout={() => setIsAuthed(false)} />
      )}

      <Box sx={{ flex: 1, minHeight: 0, overflow: "auto" }}>
        <Routes>
          <Route
            path="/login"
            element={
              isAuthed ? (
                <Navigate to="/tutors" replace />
              ) : (
                <LoginPage
                  themeMode={themeMode}
                  toggleTheme={toggleTheme}
                  onLogin={() => setIsAuthed(true)}
                />
              )
            }
          />

          <Route
            path="/register"
            element={
              isAuthed ? (
                <Navigate to="/tutors" replace />
              ) : (
                <RegisterPage
                  themeMode={themeMode}
                  toggleTheme={toggleTheme}
                  onRegister={() => setIsAuthed(true)}
                />
              )
            }
          />

          <Route
            path="/tutors"
            element={isAuthed ? <TutorsPage themeMode={themeMode} toggleTheme={toggleTheme} /> : <Navigate to="/login" replace />}
          />

          <Route
            path="/tutors/:id"
            element={isAuthed ? <TutorDetailsPage /> : <Navigate to="/login" replace />}
          />

          <Route
            path="/tutor-panel"
            element={isAuthed ? <TutorPanelPage /> : <Navigate to="/login" replace />}
          />

          <Route
            path="/my-bookings"
            element={isAuthed ? <MyBookingsPage /> : <Navigate to="/login" replace />}
          />

          <Route path="*" element={<Navigate to={isAuthed ? "/tutors" : "/login"} replace />} />
          <Route path="/privacy" element={<PrivacyPage />} />
<Route path="/terms" element={<TermsPage />} />
<Route path="/contact" element={<ContactPage />} />

        </Routes>
      </Box>

      {isAuthed && <AppFooter />}
    </Box>
  );
}
