import { useEffect, useMemo, useState } from "react";
import { Link as RouterLink } from "react-router-dom";
import {
  Alert,
  Box,
  Button,
  Card,
  CardContent,
  CircularProgress,
  Container,
  Divider,
  Snackbar,
  Stack,
  Typography,
  useTheme,
} from "@mui/material";
import { http } from "../api/http";

type Me = { id: string; email: string; role: string };

type MyBookingDto = {
  id: string;
  lessonOfferId: string;
  startUtc: string;
  endUtc: string;
  status: string;
};

function plStatus(status?: string | null) {
  const s = (status ?? "").trim().toLowerCase();
  if (!s) return "—";
  if (s === "paid") return "Opłacona";
  if (s === "pending") return "Oczekuje";
  if (s === "cancelled" || s === "canceled") return "Anulowana";
  if (s === "cancelledbytutor") return "Odwołana przez tutora";
  if (s === "confirmed") return "Potwierdzona";
  if (s === "completed") return "Zakończona";
  if (s === "rejected") return "Odrzucona";
  return status ?? "—";
}

function isPaidStatus(status?: string | null) {
  return (status ?? "").trim().toLowerCase() === "paid";
}

function isCancelledStatus(status?: string | null) {
  const s = (status ?? "").trim().toLowerCase();
  return s === "cancelled" || s === "canceled" || s === "cancelledbytutor";
}

function fmtRangePl(startUtc: string, endUtc: string) {
  const start = new Date(startUtc);
  const end = new Date(endUtc);

  const day = start.toLocaleDateString("pl-PL", {
    weekday: "long",
    year: "numeric",
    month: "2-digit",
    day: "2-digit",
  });

  const startTime = start.toLocaleTimeString("pl-PL", { hour: "2-digit", minute: "2-digit" });
  const endTime = end.toLocaleTimeString("pl-PL", { hour: "2-digit", minute: "2-digit" });

  return { day, time: `${startTime}–${endTime}` };
}

export default function MyBookingsPage() {
  const theme = useTheme();

  const [me, setMe] = useState<Me | null>(null);

  const [items, setItems] = useState<MyBookingDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [snackOpen, setSnackOpen] = useState(false);
  const [snackMsg, setSnackMsg] = useState("");

  const loadMe = async () => {
    const res = await http.get<Me>("/api/auth/me");
    setMe(res.data);
    return res.data;
  };

  const load = async () => {
    setLoading(true);
    setError(null);
    try {
      const who = me ?? (await loadMe());
      const url = who.role === "Tutor" ? "/api/bookings/mine-as-tutor" : "/api/bookings/mine";
      const res = await http.get<MyBookingDto[]>(url);
      setItems(res.data);
    } catch (e: any) {
      setError(e?.response?.data?.message ?? "Nie udało się pobrać rezerwacji.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    load();
  }, []);

  const sorted = useMemo(() => {
    return [...items].sort((a, b) => new Date(b.startUtc).getTime() - new Date(a.startUtc).getTime());
  }, [items]);

  const pay = async (id: string) => {
    try {
      const res = await http.post<{ message: string }>(`/api/bookings/${id}/pay`);
      setSnackMsg(res.data?.message ?? "Zajęcia zostały opłacone");
      setSnackOpen(true);
      await load();
    } catch (e: any) {
      setSnackMsg(e?.response?.data?.message ?? "Nie udało się opłacić zajęć.");
      setSnackOpen(true);
    }
  };

  const cancelBooking = async (id: string) => {
    try {
      const res = await http.post<{ message: string }>(`/api/bookings/${id}/cancel`);
      setSnackMsg(res.data?.message ?? "Rezerwacja została anulowana");
      setSnackOpen(true);
      await load();
    } catch (e: any) {
      setSnackMsg(e?.response?.data?.message ?? "Nie udało się anulować rezerwacji.");
      setSnackOpen(true);
    }
  };

  const cancelAsTutor = async (id: string) => {
    try {
      const res = await http.post<{ message: string }>(`/api/bookings/${id}/cancel-as-tutor`);
      setSnackMsg(res.data?.message ?? "Zajęcia zostały odwołane przez tutora");
      setSnackOpen(true);
      await load();
    } catch (e: any) {
      setSnackMsg(e?.response?.data?.message ?? "Nie udało się odwołać zajęć.");
      setSnackOpen(true);
    }
  };

  const statusPillSx = (status?: string | null) => {
    const s = (status ?? "").trim().toLowerCase();

    if (s === "paid") {
      return {
        bgcolor: "rgba(30, 58, 95, 0.12)",
        color: theme.palette.primary.dark,
        border: `1px solid rgba(30, 58, 95, 0.35)`,
      };
    }

    if (s === "pending") {
      return {
        bgcolor: "rgba(242, 159, 5, 0.16)",
        color: theme.palette.secondary.dark,
        border: `1px solid rgba(242, 159, 5, 0.45)`,
      };
    }

    if (s === "cancelled" || s === "canceled" || s === "cancelledbytutor" || s === "rejected") {
      return {
        bgcolor: "rgba(20, 42, 68, 0.10)",
        color: theme.palette.text.primary,
        border: `1px solid ${theme.palette.divider}`,
      };
    }

    return {
      bgcolor: "rgba(30, 58, 95, 0.10)",
      color: theme.palette.primary.main,
      border: `1px solid rgba(30, 58, 95, 0.28)`,
    };
  };

  return (
    <Box sx={{ py: 3 }}>
      <Container maxWidth="md">
        <Stack spacing={2}>
          <Card
            sx={{
              border: "1px solid",
              borderColor: "divider",
              boxShadow: "0 10px 28px rgba(0,0,0,0.06)",
            }}
          >
            <CardContent>
              <Stack
                direction={{ xs: "column", sm: "row" }}
                spacing={1.5}
                alignItems={{ sm: "center" }}
                justifyContent="space-between"
              >
                <Box>
                  <Typography variant="h5" fontWeight={1000} sx={{ letterSpacing: -0.2 }}>
                    Moje rezerwacje
                  </Typography>
                  <Typography variant="body2" color="text.secondary">
                    Lista Twoich rezerwacji i płatności
                  </Typography>
                </Box>

                <Button component={RouterLink} to="/tutors" variant="outlined">
                  Wróć do ofert
                </Button>
              </Stack>
            </CardContent>
          </Card>

          {loading && (
            <Box sx={{ display: "flex", justifyContent: "center", py: 6 }}>
              <CircularProgress />
            </Box>
          )}

          {!loading && error && <Alert severity="error">{error}</Alert>}

          {!loading && !error && sorted.length === 0 && (
            <Alert severity="info">Nie masz jeszcze żadnych rezerwacji.</Alert>
          )}

          {!loading &&
            !error &&
            sorted.map((b) => {
              const paid = isPaidStatus(b.status);
              const cancelled = isCancelledStatus(b.status);
              const { day, time } = fmtRangePl(b.startUtc, b.endUtc);

              return (
                <Card
                  key={b.id}
                  sx={{
                    border: "1px solid",
                    borderColor: "divider",
                    boxShadow: "0 10px 28px rgba(0,0,0,0.06)",
                    transition: "transform 140ms ease, box-shadow 140ms ease, border-color 140ms ease",
                    "&:hover": {
                      transform: "translateY(-1px)",
                      boxShadow: "0 16px 36px rgba(0,0,0,0.08)",
                      borderColor: "rgba(30, 58, 95, 0.55)",
                    },
                  }}
                >
                  <CardContent>
                    <Stack spacing={1.25}>
                      <Stack direction="row" spacing={2} alignItems="flex-start">
                        <Box sx={{ minWidth: 0 }}>
                          <Typography fontWeight={1000} sx={{ letterSpacing: -0.2 }}>
                            Rezerwacja
                          </Typography>

                          <Stack spacing={0.2} mt={0.4}>
                            <Typography
                              sx={{
                                fontWeight: 1000,
                                fontSize: 15.5,
                                color: "primary.main",
                                letterSpacing: -0.1,
                              }}
                            >
                              {day}
                            </Typography>
                            <Typography sx={{ fontWeight: 900, fontSize: 18, lineHeight: 1.1 }}>
                              {time}
                            </Typography>
                          </Stack>
                        </Box>

                        <Box sx={{ flex: 1 }} />

                        <Box
                          sx={{
                            px: 1.25,
                            py: 0.55,
                            borderRadius: 999,
                            fontWeight: 900,
                            fontSize: 12.5,
                            letterSpacing: 0.2,
                            whiteSpace: "nowrap",
                            ...statusPillSx(b.status),
                          }}
                        >
                          {plStatus(b.status)}
                        </Box>
                      </Stack>

                      <Divider />

                      <Stack direction={{ xs: "column", sm: "row" }} spacing={1.5} alignItems={{ sm: "center" }}>
                        <Typography variant="body2" color="text.secondary">
                          Status: <b style={{ color: theme.palette.text.primary }}>{plStatus(b.status)}</b>
                        </Typography>

                        <Box sx={{ flex: 1 }} />

                        {me?.role !== "Tutor" && !paid && !cancelled && (
                          <Stack direction="row" spacing={1}>
                            <Button variant="outlined" color="error" onClick={() => cancelBooking(b.id)}>
                              Anuluj
                            </Button>
                            <Button variant="contained" onClick={() => pay(b.id)}>
                              Zapłać
                            </Button>
                          </Stack>
                        )}

                        {me?.role === "Tutor" && !cancelled && (
                          <Button variant="outlined" color="error" onClick={() => cancelAsTutor(b.id)}>
                            Odwołaj
                          </Button>
                        )}
                      </Stack>
                    </Stack>
                  </CardContent>
                </Card>
              );
            })}
        </Stack>
      </Container>

      <Snackbar
        open={snackOpen}
        autoHideDuration={3500}
        onClose={() => setSnackOpen(false)}
        anchorOrigin={{ vertical: "top", horizontal: "center" }}
      >
        <Alert
          severity={snackMsg === "Zajęcia zostały opłacone" ? "success" : "error"}
          onClose={() => setSnackOpen(false)}
          sx={{ width: "100%" }}
        >
          {snackMsg}
        </Alert>
      </Snackbar>
    </Box>
  );
}
