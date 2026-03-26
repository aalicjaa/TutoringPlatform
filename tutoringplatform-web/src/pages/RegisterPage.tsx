import { useMemo, useState } from "react";
import { useNavigate } from "react-router-dom";
import {
  Alert,
  Box,
  Button,
  Container,
  Grid,
  IconButton,
  MenuItem,
  Paper,
  Stack,
  TextField,
  Tooltip,
  Typography,
} from "@mui/material";
import type { PaletteMode } from "@mui/material";
import DarkModeOutlinedIcon from "@mui/icons-material/DarkModeOutlined";
import LightModeOutlinedIcon from "@mui/icons-material/LightModeOutlined";
import { http, tokenStorage } from "../api/http";

type Props = {
  themeMode: PaletteMode;
  toggleTheme: () => void;
  onRegister: () => void;
};

type RegisterResponse = {
  token: string;
  email: string;
  role: string;
};

export default function RegisterPage({ themeMode, toggleTheme, onRegister }: Props) {
  const navigate = useNavigate();

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("Password1");
  const [confirmPassword, setConfirmPassword] = useState("Password1");
  const [role, setRole] = useState<"Student" | "Tutor">("Student");

  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const canSubmit = useMemo(() => {
    if (!email.trim()) return false;
    if (password.length < 6) return false;
    if (password !== confirmPassword) return false;
    return true;
  }, [email, password, confirmPassword]);

  const submit = async () => {
    setError(null);

    const e = email.trim();
    if (!e) {
      setError("Podaj email.");
      return;
    }
    if (password !== confirmPassword) {
      setError("Hasła nie są takie same.");
      return;
    }

    setLoading(true);
    try {
      const res = await http.post<RegisterResponse>("/api/auth/register", {
        email: e,
        password,
        role,
      });

      tokenStorage.set(res.data.token);
      onRegister();
      navigate("/tutors");
    } catch (err: any) {
      setError(err?.response?.data?.message ?? "Nie udało się zarejestrować.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <Box sx={{ py: { xs: 6, md: 8 }, minHeight: "100vh" }}>
      <Container maxWidth="lg">
        <Grid container spacing={4} alignItems="center">
          <Grid item xs={12} md={6}>
            <Stack spacing={2.5}>
              <Typography variant="h3" fontWeight={1000}>
                Zarejestruj konto
              </Typography>

              <Typography color="text.secondary">
                Utwórz konto ucznia lub tutora. Po rejestracji możesz od razu korzystać z aplikacji.
              </Typography>

              <Stack direction="row" spacing={1}>
                <Button variant="outlined" onClick={() => navigate("/login")}>
                  Mam konto
                </Button>
                <Button variant="contained" onClick={submit} disabled={!canSubmit || loading}>
                  Utwórz konto
                </Button>
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
                    Rejestracja
                  </Typography>

                  <Tooltip title={themeMode === "dark" ? "Jasny motyw" : "Ciemny motyw"}>
                    <IconButton onClick={toggleTheme}>
                      {themeMode === "dark" ? <LightModeOutlinedIcon /> : <DarkModeOutlinedIcon />}
                    </IconButton>
                  </Tooltip>
                </Stack>

                {error && <Alert severity="error">{error}</Alert>}

                <TextField label="Email" value={email} onChange={(e) => setEmail(e.target.value)} />

                <TextField
                  select
                  label="Rola"
                  value={role}
                  onChange={(e) => setRole(e.target.value as "Student" | "Tutor")}
                >
                  <MenuItem value="Student">Uczeń</MenuItem>
                  <MenuItem value="Tutor">Tutor</MenuItem>
                </TextField>

                <TextField
                  label="Hasło"
                  type="password"
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                />

                <TextField
                  label="Powtórz hasło"
                  type="password"
                  value={confirmPassword}
                  onChange={(e) => setConfirmPassword(e.target.value)}
                />

                <Button variant="contained" onClick={submit} disabled={!canSubmit || loading}>
                  Zarejestruj
                </Button>

                <Typography variant="body2" color="text.secondary">
                  Masz konto?{" "}
                  <Box
                    component="span"
                    role="button"
                    tabIndex={0}
                    onClick={() => navigate("/login")}
                    sx={{ color: "secondary.main", fontWeight: 1000, cursor: "pointer" }}
                  >
                    Zaloguj się
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
