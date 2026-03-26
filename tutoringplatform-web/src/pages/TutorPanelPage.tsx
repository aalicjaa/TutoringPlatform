import { useEffect, useMemo, useState } from "react";
import {
  Alert,
  Box,
  Button,
  Container,
  Divider,
  MenuItem,
  Paper,
  Stack,
  Tab,
  Tabs,
  TextField,
  Typography,
} from "@mui/material";
import { http } from "../api/http";

type Me = { id: string; email: string; role: string };

type Subject = { id: number; name: string };

type Offer = {
  id: string;
  subjectId: number;
  subjectName: string;
  durationMinutes: number;
  price: number;
  mode: string;
};

type Availability = {
  id: string;
  startUtc: string;
  endUtc: string;
};

type MyTutorProfileDto = {
  tutorProfileId: string;
  displayName: string;
  city: string;
  bio: string;
  hourlyRate: number;
};

function toUtcIso(datetimeLocal: string) {
  const d = new Date(datetimeLocal);
  return d.toISOString();
}

export default function TutorPanelPage() {
  const [tab, setTab] = useState(0);
  const [me, setMe] = useState<Me | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [info, setInfo] = useState<string | null>(null);

  const [subjects, setSubjects] = useState<Subject[]>([]);
  const [offers, setOffers] = useState<Offer[]>([]);
  const [availability, setAvailability] = useState<Availability[]>([]);

  const [offerSubjectId, setOfferSubjectId] = useState<number>(1);
  const [offerDuration, setOfferDuration] = useState<number>(60);
  const [offerPrice, setOfferPrice] = useState<number>(100);
  const [offerMode, setOfferMode] = useState<string>("Online");

  const [rangeFrom, setRangeFrom] = useState<string>(() => {
    const d = new Date();
    d.setHours(0, 0, 0, 0);
    return d.toISOString().slice(0, 16);
  });
  const [rangeTo, setRangeTo] = useState<string>(() => {
    const d = new Date();
    d.setDate(d.getDate() + 14);
    d.setHours(23, 59, 0, 0);
    return d.toISOString().slice(0, 16);
  });

  const [slotStart, setSlotStart] = useState<string>(() => {
    const d = new Date();
    d.setHours(d.getHours() + 2, 0, 0, 0);
    return d.toISOString().slice(0, 16);
  });
  const [slotEnd, setSlotEnd] = useState<string>(() => {
    const d = new Date();
    d.setHours(d.getHours() + 3, 0, 0, 0);
    return d.toISOString().slice(0, 16);
  });

  const [profile, setProfile] = useState<MyTutorProfileDto | null>(null);
  const [profileLoading, setProfileLoading] = useState(false);

  const [displayName, setDisplayName] = useState("");
  const [city, setCity] = useState("");
  const [bio, setBio] = useState("");
  const [hourlyRate, setHourlyRate] = useState<number>(100);

  const isTutor = useMemo(() => me?.role === "Tutor", [me]);
  const hasProfile = !!profile?.tutorProfileId;

  const loadMe = async () => {
    const res = await http.get<Me>("/api/auth/me");
    setMe(res.data);
  };

  const loadSubjects = async () => {
    const res = await http.get<Subject[]>("/api/subjects");
    setSubjects(res.data);
    if (res.data.length > 0) setOfferSubjectId(res.data[0].id);
  };

  const loadOffers = async () => {
    const res = await http.get<Offer[]>("/api/my-tutor/offers");
    setOffers(res.data);
  };

  const loadAvailability = async () => {
    const res = await http.get<Availability[]>("/api/my-tutor/availability", {
      params: {
        fromUtc: toUtcIso(rangeFrom),
        toUtc: toUtcIso(rangeTo),
      },
    });
    setAvailability(res.data);
  };

  const loadProfile = async () => {
    setProfileLoading(true);
    try {
      const res = await http.get<MyTutorProfileDto>("/api/my-tutor/profile");
      setProfile(res.data);
      setDisplayName(res.data.displayName ?? "");
      setCity(res.data.city ?? "");
      setBio(res.data.bio ?? "");
      setHourlyRate(Number(res.data.hourlyRate ?? 0));
    } catch (e: any) {
      const msg = e?.response?.data?.message ?? null;
      setProfile(null);
      setDisplayName("");
      setCity("");
      setBio("");
      setHourlyRate(100);
      if (msg) setError(msg);
    } finally {
      setProfileLoading(false);
    }
  };

  useEffect(() => {
    setError(null);
    setInfo(null);
    loadMe()
      .then(() => loadSubjects())
      .catch(() => setError("Brak autoryzacji. Zaloguj się jako Tutor."));
  }, []);

  useEffect(() => {
    if (!isTutor) return;
    loadProfile().catch(() => {});
    loadOffers().catch(() => {});
    loadAvailability().catch(() => {});
  }, [isTutor]);

  const saveProfile = async () => {
    setError(null);
    setInfo(null);

    const dto = {
      displayName: displayName.trim(),
      city: city.trim(),
      bio: bio.trim(),
      hourlyRate: Number(hourlyRate),
    };

    if (!dto.displayName) {
      setError("DisplayName jest wymagane.");
      return;
    }
    if (!dto.city) {
      setError("City jest wymagane.");
      return;
    }
    if (Number.isNaN(dto.hourlyRate) || dto.hourlyRate < 0) {
      setError("HourlyRate nie może być ujemne.");
      return;
    }

    try {
      if (hasProfile) {
        const res = await http.put<MyTutorProfileDto>("/api/my-tutor/profile", dto);
        setProfile(res.data);
        setInfo("Zapisano profil.");
      } else {
        const res = await http.post<MyTutorProfileDto>("/api/my-tutor/profile", dto);
        setProfile(res.data);
        setInfo("Utworzono profil.");
      }
    } catch (e: any) {
      setError(e?.response?.data?.message ?? "Nie udało się zapisać profilu.");
    }
  };

  const createOffer = async () => {
    setError(null);
    setInfo(null);
    try {
      await http.post("/api/my-tutor/offers", {
        subjectId: offerSubjectId,
        durationMinutes: offerDuration,
        price: offerPrice,
        mode: offerMode,
      });
      await loadOffers();
      setInfo("Dodano ofertę.");
    } catch (e: any) {
      setError(e?.response?.data?.message ?? "Nie udało się dodać oferty.");
    }
  };

  const deleteOffer = async (id: string) => {
    setError(null);
    setInfo(null);
    try {
      await http.delete(`/api/my-tutor/offers/${id}`);
      await loadOffers();
      setInfo("Usunięto ofertę.");
    } catch (e: any) {
      setError(e?.response?.data?.message ?? "Nie udało się usunąć oferty.");
    }
  };

  const createSlot = async () => {
    setError(null);
    setInfo(null);
    try {
      await http.post("/api/my-tutor/availability", {
        startUtc: toUtcIso(slotStart),
        endUtc: toUtcIso(slotEnd),
      });
      await loadAvailability();
      setInfo("Dodano dostępność.");
    } catch (e: any) {
      setError(e?.response?.data?.message ?? "Nie udało się dodać dostępności.");
    }
  };

  const deleteSlot = async (id: string) => {
    setError(null);
    setInfo(null);
    try {
      await http.delete(`/api/my-tutor/availability/${id}`);
      await loadAvailability();
      setInfo("Usunięto dostępność.");
    } catch (e: any) {
      setError(e?.response?.data?.message ?? "Nie udało się usunąć dostępności.");
    }
  };

  const reloadAvailability = async () => {
    setError(null);
    setInfo(null);
    try {
      await loadAvailability();
    } catch (e: any) {
      setError(e?.response?.data?.message ?? "Nie udało się pobrać dostępności.");
    }
  };

  return (
    <Container maxWidth="lg">
      <Stack spacing={2} mt={4}>
        <Paper sx={{ p: 2 }}>
          <Stack direction="row" justifyContent="space-between" alignItems="center">
            <Box>
              <Typography variant="h5" fontWeight={900}>
                Panel tutora
              </Typography>
              <Typography color="text.secondary">{me ? me.email : "—"}</Typography>
            </Box>
          </Stack>
        </Paper>

        {error && <Alert severity="error">{error}</Alert>}
        {info && <Alert severity="success">{info}</Alert>}

        {!isTutor ? (
          <Paper sx={{ p: 2 }}>
            <Typography>Zaloguj się kontem z rolą Tutor, żeby korzystać z panelu.</Typography>
          </Paper>
        ) : (
          <Paper sx={{ p: 0 }}>
            <Tabs
              value={tab}
              onChange={(_, v) => setTab(v)}
              variant="fullWidth"
              sx={{
                "& .MuiTab-root": { fontWeight: 900, opacity: 0.72 },
                "& .Mui-selected": { opacity: 1 },
              }}
            >
              <Tab label="Oferty" />
              <Tab label="Dostępność" />
              <Tab label="Profil" />
            </Tabs>

            <Divider />

            {tab === 0 && (
              <Box p={2}>
                <Stack spacing={2}>
                  <Typography fontWeight={900}>Dodaj ofertę</Typography>

                  <Stack direction={{ xs: "column", sm: "row" }} spacing={2}>
                    <TextField
                      select
                      label="Przedmiot"
                      value={offerSubjectId}
                      onChange={(e) => setOfferSubjectId(Number(e.target.value))}
                      fullWidth
                      disabled={!hasProfile}
                    >
                      {subjects.map((s) => (
                        <MenuItem key={s.id} value={s.id}>
                          {s.name}
                        </MenuItem>
                      ))}
                    </TextField>

                    <TextField
                      label="Czas (min)"
                      type="number"
                      value={offerDuration}
                      onChange={(e) => setOfferDuration(Number(e.target.value))}
                      fullWidth
                      disabled={!hasProfile}
                    />

                    <TextField
                      label="Cena (zł)"
                      type="number"
                      value={offerPrice}
                      onChange={(e) => setOfferPrice(Number(e.target.value))}
                      fullWidth
                      disabled={!hasProfile}
                    />

                    <TextField
                      select
                      label="Tryb"
                      value={offerMode}
                      onChange={(e) => setOfferMode(e.target.value)}
                      fullWidth
                      disabled={!hasProfile}
                    >
                      <MenuItem value="Online">Online</MenuItem>
                      <MenuItem value="Onsite">Onsite</MenuItem>
                    </TextField>

                    <Button variant="contained" onClick={createOffer} sx={{ minWidth: 160 }} disabled={!hasProfile}>
                      Dodaj
                    </Button>
                  </Stack>

                  {!hasProfile && (
                    <Alert severity="info">
                      Najpierw uzupełnij profil w zakładce „Profil”, aby dodawać oferty i dostępność.
                    </Alert>
                  )}

                  <Divider />

                  <Typography fontWeight={900}>Moje oferty</Typography>

                  {offers.length === 0 ? (
                    <Typography color="text.secondary">Brak ofert.</Typography>
                  ) : (
                    <Stack spacing={1}>
                      {offers.map((o) => (
                        <Paper key={o.id} sx={{ p: 2 }}>
                          <Stack direction={{ xs: "column", sm: "row" }} spacing={1} justifyContent="space-between">
                            <Box>
                              <Typography fontWeight={900}>{o.subjectName}</Typography>
                              <Typography color="text.secondary">
                                {o.durationMinutes} min • {o.mode}
                              </Typography>
                              <Typography>{o.price} zł</Typography>
                            </Box>
                            <Button color="error" variant="outlined" onClick={() => deleteOffer(o.id)}>
                              Usuń
                            </Button>
                          </Stack>
                        </Paper>
                      ))}
                    </Stack>
                  )}
                </Stack>
              </Box>
            )}

            {tab === 1 && (
              <Box p={2}>
                <Stack spacing={2}>
                  <Typography fontWeight={900}>Zakres podglądu</Typography>

                  <Stack direction={{ xs: "column", sm: "row" }} spacing={2}>
                    <TextField
                      label="Od"
                      type="datetime-local"
                      value={rangeFrom}
                      onChange={(e) => setRangeFrom(e.target.value)}
                      InputLabelProps={{ shrink: true }}
                      fullWidth
                      disabled={!hasProfile}
                    />
                    <TextField
                      label="Do"
                      type="datetime-local"
                      value={rangeTo}
                      onChange={(e) => setRangeTo(e.target.value)}
                      InputLabelProps={{ shrink: true }}
                      fullWidth
                      disabled={!hasProfile}
                    />
                    <Button variant="outlined" onClick={reloadAvailability} sx={{ minWidth: 160 }} disabled={!hasProfile}>
                      Odśwież
                    </Button>
                  </Stack>

                  {!hasProfile && (
                    <Alert severity="info">
                      Najpierw uzupełnij profil w zakładce „Profil”, aby dodawać oferty i dostępność.
                    </Alert>
                  )}

                  <Divider />

                  <Typography fontWeight={900}>Dodaj dostępność</Typography>

                  <Stack direction={{ xs: "column", sm: "row" }} spacing={2}>
                    <TextField
                      label="Start"
                      type="datetime-local"
                      value={slotStart}
                      onChange={(e) => setSlotStart(e.target.value)}
                      InputLabelProps={{ shrink: true }}
                      fullWidth
                      disabled={!hasProfile}
                    />
                    <TextField
                      label="Koniec"
                      type="datetime-local"
                      value={slotEnd}
                      onChange={(e) => setSlotEnd(e.target.value)}
                      InputLabelProps={{ shrink: true }}
                      fullWidth
                      disabled={!hasProfile}
                    />
                    <Button variant="contained" onClick={createSlot} sx={{ minWidth: 160 }} disabled={!hasProfile}>
                      Dodaj
                    </Button>
                  </Stack>

                  <Divider />

                  <Typography fontWeight={900}>Moja dostępność</Typography>

                  {availability.length === 0 ? (
                    <Typography color="text.secondary">Brak slotów w wybranym zakresie.</Typography>
                  ) : (
                    <Stack spacing={1}>
                      {availability.map((a) => (
                        <Paper key={a.id} sx={{ p: 2 }}>
                          <Stack direction={{ xs: "column", sm: "row" }} spacing={1} justifyContent="space-between">
                            <Box>
                              <Typography fontWeight={900}>{new Date(a.startUtc).toLocaleString()}</Typography>
                              <Typography color="text.secondary">{new Date(a.endUtc).toLocaleString()}</Typography>
                            </Box>
                            <Button color="error" variant="outlined" onClick={() => deleteSlot(a.id)} disabled={!hasProfile}>
                              Usuń
                            </Button>
                          </Stack>
                        </Paper>
                      ))}
                    </Stack>
                  )}
                </Stack>
              </Box>
            )}

            {tab === 2 && (
              <Box p={2}>
                <Stack spacing={2}>
                  <Typography fontWeight={900}>Mój profil</Typography>

                  <Typography color="text.secondary">
                    Wpisz nazwę, pod jaką chcesz być widoczna/y w ofertach oraz podstawowe informacje.
                  </Typography>

                  <Stack direction={{ xs: "column", sm: "row" }} spacing={2}>
                    <TextField
                      label="Imię i nazwisko / nazwa"
                      value={displayName}
                      onChange={(e) => setDisplayName(e.target.value)}
                      fullWidth
                      disabled={profileLoading}
                    />
                    <TextField
                      label="Miasto"
                      value={city}
                      onChange={(e) => setCity(e.target.value)}
                      fullWidth
                      disabled={profileLoading}
                    />
                  </Stack>

                  <TextField
                    label="Opis"
                    value={bio}
                    onChange={(e) => setBio(e.target.value)}
                    fullWidth
                    multiline
                    minRows={4}
                    disabled={profileLoading}
                  />

                  <TextField
                    label="Stawka godzinowa (zł)"
                    type="number"
                    value={hourlyRate}
                    onChange={(e) => setHourlyRate(Number(e.target.value))}
                    fullWidth
                    disabled={profileLoading}
                  />

                  <Stack direction="row" spacing={1}>
                    <Button variant="contained" onClick={saveProfile} disabled={profileLoading}>
                      {hasProfile ? "Zapisz zmiany" : "Utwórz profil"}
                    </Button>

                    <Button
                      variant="outlined"
                      onClick={() => {
                        setError(null);
                        setInfo(null);
                        loadProfile().catch(() => {});
                      }}
                      disabled={profileLoading}
                    >
                      Odśwież
                    </Button>
                  </Stack>
                </Stack>
              </Box>
            )}
          </Paper>
        )}
      </Stack>
    </Container>
  );
}
