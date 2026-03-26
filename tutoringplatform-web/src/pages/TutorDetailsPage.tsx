import { useEffect, useMemo, useState } from "react";
import { Link as RouterLink, useParams } from "react-router-dom";
import {
  Alert,
  Box,
  Button,
  Chip,
  Container,
  Divider,
  Paper,
  Stack,
  TextField,
  Typography,
} from "@mui/material";
import { http } from "../api/http";

type Subject = { id: number; name: string };

type Offer = {
  id: string;
  subjectId: number;
  subjectName: string;
  durationMinutes: number;
  price: number;
  mode: string;
};

type TutorDetails = {
  id: string;
  displayName: string;
  bio: string;
  city: string;
  hourlyRate: number;
  minOfferPrice?: number | null;
  subjects: Subject[];
  offers: Offer[];
};

type Availability = {
  id: string;
  startUtc: string;
  endUtc: string;
};

function fmtLocal(iso: string) {
  return new Date(iso).toLocaleString("pl-PL", {
    weekday: "short",
    year: "numeric",
    month: "2-digit",
    day: "2-digit",
    hour: "2-digit",
    minute: "2-digit",
  });
}

function fmtTime(iso: string) {
  return new Date(iso).toLocaleTimeString("pl-PL", { hour: "2-digit", minute: "2-digit" });
}

function fmtDateOnly(iso: string) {
  return new Date(iso).toLocaleDateString("pl-PL", {
    weekday: "long",
    year: "numeric",
    month: "long",
    day: "2-digit",
  });
}

function addDaysUtc(days: number) {
  const d = new Date();
  // bierzemy "teraz" w UTC jako ISO
  const from = new Date(d.getTime());
  const to = new Date(d.getTime() + days * 24 * 60 * 60 * 1000);
  return { fromUtc: from.toISOString(), toUtc: to.toISOString() };
}

export default function TutorDetailsPage() {
  const { id } = useParams<{ id: string }>();

  const [tutor, setTutor] = useState<TutorDetails | null>(null);

  const [availability, setAvailability] = useState<Availability[]>([]);
  const [selectedOfferId, setSelectedOfferId] = useState<string>("");
  const [selectedSlot, setSelectedSlot] = useState<Availability | null>(null);


  const [daysAhead, setDaysAhead] = useState<number>(14);

  const [date, setDate] = useState<string>(() => {
    const d = new Date();
    return d.toISOString().slice(0, 10);
  });

  const [info, setInfo] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [loadingSlots, setLoadingSlots] = useState(false);

  const hasOffers = (tutor?.offers?.length ?? 0) > 0;

  const selectedOffer = useMemo(() => {
    if (!tutor) return null;
    return tutor.offers.find((o) => o.id === selectedOfferId) ?? null;
  }, [tutor, selectedOfferId]);

  const loadTutor = async () => {
    if (!id) return;
    const res = await http.get<TutorDetails>(`/api/tutors/${id}`);
    setTutor(res.data);
    const firstOfferId = res.data.offers?.[0]?.id ?? "";
    setSelectedOfferId(firstOfferId);
  };

  const loadAvailabilityRange = async (rangeDays: number) => {
    if (!id) return;
    setLoadingSlots(true);
    try {
      const { fromUtc, toUtc } = addDaysUtc(rangeDays);
      const res = await http.get<Availability[]>(`/api/AvailabilitySlots`, {
        params: {
          tutorProfileId: id,
          fromUtc,
          toUtc,
        },
      });
      setAvailability(res.data);
      setSelectedSlot(null);
    } finally {
      setLoadingSlots(false);
    }
  };

  const loadAvailabilityForDay = async (dayIso: string) => {
    if (!id) return;
    setLoadingSlots(true);
    try {
      // dzień lokalny użytkownika → zakres UTC na podstawie tego dnia
      const d = new Date(dayIso);
      const start = new Date(Date.UTC(d.getUTCFullYear(), d.getUTCMonth(), d.getUTCDate(), 0, 0, 0));
      const end = new Date(Date.UTC(d.getUTCFullYear(), d.getUTCMonth(), d.getUTCDate() + 1, 0, 0, 0));

      const res = await http.get<Availability[]>(`/api/AvailabilitySlots`, {
        params: {
          tutorProfileId: id,
          fromUtc: start.toISOString(),
          toUtc: end.toISOString(),
        },
      });
      setAvailability(res.data);
      setSelectedSlot(null);
    } finally {
      setLoadingSlots(false);
    }
  };

  useEffect(() => {
    setError(null);
    setInfo(null);
    if (!id) return;

    loadTutor().catch(() => setError("Nie udało się pobrać danych tutora."));
    // od razu ładujemy listę terminów (14 dni)
    loadAvailabilityRange(14).catch(() => setError("Nie udało się pobrać dostępności."));
    setDaysAhead(14);
  }, [id]);

  // grupowanie slotów po dniach (czytelny marketplace)
  const grouped = useMemo(() => {
    const map = new Map<string, Availability[]>();
    [...availability]
      .sort((a, b) => new Date(a.startUtc).getTime() - new Date(b.startUtc).getTime())
      .forEach((slot) => {
        const key = new Date(slot.startUtc).toISOString().slice(0, 10); // yyyy-mm-dd
        const arr = map.get(key) ?? [];
        arr.push(slot);
        map.set(key, arr);
      });
    return Array.from(map.entries()); // [dayIso, slots]
  }, [availability]);

  const book = async () => {
    setError(null);
    setInfo(null);

    if (!selectedOfferId) {
      setError("Wybierz ofertę.");
      return;
    }
    if (!selectedSlot) {
      setError("Wybierz termin.");
      return;
    }

    try {
      await http.post(`/api/Bookings`, {
        dto: {
          lessonOfferId: selectedOfferId,
          startUtc: selectedSlot.startUtc,
          endUtc: selectedSlot.endUtc,
        },
      });
      setInfo("Rezerwacja utworzona.");
      // odśwież listę terminów (żeby zniknął zajęty slot)
      await loadAvailabilityRange(daysAhead);
    } catch (e: any) {
      setError(e?.response?.data?.message ?? "Nie udało się utworzyć rezerwacji.");
    }
  };

  return (
    <Container maxWidth="md">
      <Stack spacing={2} mt={4} mb={4}>
        <Stack direction="row" justifyContent="flex-end">
          <Button component={RouterLink} to="/my-bookings" variant="outlined">
            Moje rezerwacje
          </Button>
        </Stack>

        {error && <Alert severity="error">{error}</Alert>}
        {info && <Alert severity="success">{info}</Alert>}

        {/* Profil tutora */}
        <Paper sx={{ p: 2 }}>
          <Stack spacing={1}>
            <Stack direction="row" justifyContent="space-between" alignItems="baseline">
              <Typography variant="h5" fontWeight={900}>
                {tutor?.displayName ?? "—"}
              </Typography>
              <Chip
                label={tutor?.minOfferPrice ? `Od ${tutor.minOfferPrice} zł` : "Brak ofert"}
                color={tutor?.minOfferPrice ? "secondary" : "default"}
                size="small"
              />
            </Stack>

            <Typography color="text.secondary">{tutor?.city ?? "—"}</Typography>
            <Typography>{tutor?.bio ?? "—"}</Typography>

            <Typography color="text.secondary">
              Przedmioty: {(tutor?.subjects ?? []).map((s) => s.name).join(", ") || "—"}
            </Typography>
          </Stack>
        </Paper>

        {/* Oferty */}
        <Paper sx={{ p: 2 }}>
          <Stack spacing={2}>
            <Typography fontWeight={900}>Oferta</Typography>

            {!hasOffers ? (
              <Alert severity="info">Brak ofert dla tego korepetytora.</Alert>
            ) : (
              <TextField
                select
                label="Wybierz ofertę"
                value={selectedOfferId}
                onChange={(e) => setSelectedOfferId(e.target.value)}
                fullWidth
                SelectProps={{ native: true }}
              >
                {(tutor?.offers ?? []).map((o) => (
                  <option key={o.id} value={o.id}>
                    {o.subjectName} • {o.durationMinutes} min • {o.mode} • {o.price} zł
                  </option>
                ))}
              </TextField>
            )}

        
          </Stack>
        </Paper>

        {/* Terminy */}
        <Paper sx={{ p: 2 }}>
          <Stack spacing={2}>
            <Stack direction={{ xs: "column", sm: "row" }} spacing={1} justifyContent="space-between" alignItems={{ sm: "center" }}>
              <Box>
           <Box>
  <Typography fontWeight={900}>Dostępne terminy</Typography>
  <Box
    sx={{
      mt: 0.5,
      height: 3,
      width: 48,
      borderRadius: 2,
      bgcolor: "secondary.main",
    }}
  />
</Box>

                <Typography variant="body2" color="text.secondary">
                  Poniżej widzisz najbliższe terminy — wybierz i zarezerwuj.
                </Typography>
              </Box>

              <Stack direction="row" spacing={1} alignItems="center">
                <Button
                  variant="outlined"
                  disabled={loadingSlots}
                  onClick={async () => {
                    setError(null);
                    setInfo(null);
                    setDaysAhead(14);
                    try {
                      await loadAvailabilityRange(14);
                      setInfo("Pokazano terminy z najbliższych 14 dni.");
                    } catch (e: any) {
                      setError(e?.response?.data?.message ?? "Nie udało się pobrać dostępności.");
                    }
                  }}
                >
                  Najbliższe 14 dni
                </Button>

                <Button
                  variant="outlined"
                  disabled={loadingSlots}
                  onClick={async () => {
                    setError(null);
                    setInfo(null);
                    const next = 30;
                    setDaysAhead(next);
                    try {
                      await loadAvailabilityRange(next);
                      setInfo("Pokazano terminy z najbliższych 30 dni.");
                    } catch (e: any) {
                      setError(e?.response?.data?.message ?? "Nie udało się pobrać dostępności.");
                    }
                  }}
                >
                  30 dni
                </Button>
              </Stack>
            </Stack>

            <Divider />

            {/* Opcjonalny filtr “konkretny dzień” */}
            <Stack direction={{ xs: "column", sm: "row" }} spacing={2} alignItems={{ sm: "center" }}>
              <TextField
                label="Filtruj po dacie (opcjonalnie)"
                type="date"
                value={date}
                onChange={(e) => setDate(e.target.value)}
                InputLabelProps={{ shrink: true }}
                fullWidth
              />
              <Button
                variant="contained"
                disabled={loadingSlots}
                onClick={async () => {
                  setError(null);
                  setInfo(null);
                  try {
                    await loadAvailabilityForDay(date);
                    setInfo("Pobrano dostępność dla wybranego dnia.");
                  } catch (e: any) {
                    setError(e?.response?.data?.message ?? "Nie udało się pobrać dostępności.");
                  }
                }}
                sx={{ minWidth: 160 }}
              >
                Sprawdź dzień
              </Button>
            </Stack>

            {availability.length === 0 ? (
              <Alert severity="info">
                Brak dostępnych terminów w wybranym zakresie.
              </Alert>
            ) : (
              <Stack spacing={1.5}>
                {grouped.map(([dayIso, slots]) => (
                  <Paper key={dayIso} variant="outlined" sx={{ p: 1.5, borderRadius: 2 }}>
                    <Typography fontWeight={900} sx={{ mb: 1 }}>
                      {fmtDateOnly(dayIso)}
                    </Typography>

                    <Stack direction="row" flexWrap="wrap" gap={1}>
                      {slots.map((s) => {
                        const active = selectedSlot?.id === s.id;
                        return (
                          <Chip
  key={s.id}
  label={`${fmtTime(s.startUtc)}–${fmtTime(s.endUtc)}`}
  clickable
  variant={active ? "filled" : "outlined"}
  onClick={() => setSelectedSlot(s)}
  sx={{
    fontWeight: 800,
    borderColor: active ? "secondary.main" : "divider",
    color: active ? "#fff" : "text.primary",
    bgcolor: active ? "secondary.main" : "transparent",
    transition: "all 120ms",
    "&:hover": {
      borderColor: "secondary.main",
      bgcolor: active ? "secondary.dark" : "rgba(242,159,5,0.08)",
    },
  }}
/>

                        );
                      })}
                    </Stack>
                  </Paper>
                ))}
              </Stack>
            )}

            <Button
              variant="contained"
              onClick={book}
              disabled={!hasOffers || !selectedSlot || !selectedOfferId}
            >
              Zarezerwuj wybrany termin
              {selectedSlot ? ` (${fmtLocal(selectedSlot.startUtc)})` : ""}
            </Button>
          </Stack>
        </Paper>
      </Stack>
    </Container>
  );
}
