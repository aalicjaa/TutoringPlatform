import { useEffect, useMemo, useState } from "react";
import { useNavigate } from "react-router-dom";
import {
  Alert,
  Box,
  Button,
  Container,
  Paper,
  Stack,
  TextField,
  Typography,
  IconButton,
  Tooltip,
  Grid,
} from "@mui/material";
import type { PaletteMode } from "@mui/material";
import DarkModeOutlinedIcon from "@mui/icons-material/DarkModeOutlined";
import LightModeOutlinedIcon from "@mui/icons-material/LightModeOutlined";
import SchoolOutlinedIcon from "@mui/icons-material/SchoolOutlined";
import CalendarMonthOutlinedIcon from "@mui/icons-material/CalendarMonthOutlined";
import PaymentsOutlinedIcon from "@mui/icons-material/PaymentsOutlined";
import { http, tokenStorage } from "../api/http";

type LoginResponse = {
  token: string;
  email: string;
  role: string;
};

type Props = {
  themeMode: PaletteMode;
  toggleTheme: () => void;
  onLogin: () => void;
};

export default function LoginPage({ themeMode, toggleTheme, onLogin }: Props) {
  const [email, setEmail] = useState("a@a.com");
  const [password, setPassword] = useState("Password1");
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  const navigate = useNavigate();

  const messages = useMemo(
    () => ["Nowoczesna platforma", "Rezerwuj zajęcia już w kilka sekund", "Zarządzaj nauką wygodnie i szybko"],
    []
  );

  const [msgIndex, setMsgIndex] = useState(0);
  const [fade, setFade] = useState(true);

  useEffect(() => {
    const id = setInterval(() => {
      setFade(false);
      setTimeout(() => {
        setMsgIndex((i) => (i + 1) % messages.length);
        setFade(true);
      }, 260);
    }, 5200);
    return () => clearInterval(id);
  }, [messages.length]);

  const submit = async () => {
    setError(null);
    setLoading(true);

    try {
      const res = await http.post<LoginResponse>("/api/auth/login", { email, password });
      tokenStorage.set(res.data.token);
      onLogin();
      navigate("/tutors");
    } catch {
      setError("Nieprawidłowy email lub hasło");
    } finally {
      setLoading(false);
    }
  };

  return (
    <Box sx={{ py: { xs: 6, md: 8 }, minHeight: "100vh" }}>
      <Container maxWidth="lg">
        <Grid container spacing={4} alignItems="center">
          <Grid item xs={12} md={6}>
            <Stack spacing={3}>
              <Typography
                variant="h3"
                fontWeight={1000}
                sx={{
                  transition: "opacity 220ms ease, transform 220ms ease",
                  opacity: fade ? 1 : 0,
                  transform: fade ? "translateY(0px)" : "translateY(6px)",
                }}
              >
                {messages[msgIndex]}
              </Typography>

              <Typography color="text.secondary">
                Znajdź idealnego korepetytora, porównaj oferty i zarezerwuj dogodny termin. Wszystko w jednym miejscu —
                prosto i wygodnie.
              </Typography>

              <Stack direction="row" spacing={2}>
                <Button variant="contained" size="large" onClick={submit} disabled={loading}>
                  Zaloguj się
                </Button>

                <Button variant="outlined" size="large" onClick={() => navigate("/register")}>
                  Zarejestruj się
                </Button>
              </Stack>

              <Stack spacing={2} sx={{ pt: 2 }}>
                <Stack direction="row" spacing={2} alignItems="center">
                  <SchoolOutlinedIcon color="secondary" />
                  <Typography>Setki korepetytorów i szeroki wybór przedmiotów</Typography>
                </Stack>

                <Stack direction="row" spacing={2} alignItems="center">
                  <CalendarMonthOutlinedIcon color="secondary" />
                  <Typography>Intuicyjny system rezerwacji terminów</Typography>
                </Stack>

                <Stack direction="row" spacing={2} alignItems="center">
                  <PaymentsOutlinedIcon color="secondary" />
                  <Typography>Szybkie i bezpieczne płatności online</Typography>
                </Stack>
              </Stack>
            </Stack>
          </Grid>

          <Grid item xs={12} md={6}>
            <Paper
              sx={{
                p: 3,
                borderRadius: 4,
                border: "1px solid",
                borderColor: "divider",
                boxShadow: "0 8px 24px rgba(0,0,0,0.08)",
              }}
            >
              <Stack spacing={2}>
                <Stack direction="row" alignItems="center" justifyContent="space-between">
                  <Typography variant="h6" fontWeight={900}>
                    Logowanie
                  </Typography>

                  <Tooltip title={themeMode === "dark" ? "Jasny motyw" : "Ciemny motyw"}>
                    <IconButton onClick={toggleTheme}>
                      {themeMode === "dark" ? <LightModeOutlinedIcon /> : <DarkModeOutlinedIcon />}
                    </IconButton>
                  </Tooltip>
                </Stack>

                {error && <Alert severity="error">{error}</Alert>}

                <TextField label="Email" value={email} onChange={(e) => setEmail(e.target.value)} />

                <TextField label="Hasło" type="password" value={password} onChange={(e) => setPassword(e.target.value)} />

                <Button variant="contained" onClick={submit} disabled={loading}>
                  Zaloguj
                </Button>

                <Typography variant="body2" color="text.secondary">
                  Nie masz konta?{" "}
                  <Box
                    component="span"
                    role="button"
                    tabIndex={0}
                    onClick={() => navigate("/register")}
                    sx={{ color: "secondary.main", fontWeight: 1000, cursor: "pointer" }}
                  >
                    Zarejestruj się
                  </Box>
                </Typography>
              </Stack>
            </Paper>
          </Grid>
        </Grid>
      </Container>
    </Box>
  );
}
