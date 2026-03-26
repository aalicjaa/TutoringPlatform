import { useEffect, useMemo, useState } from "react";
import {
  Alert,
  Box,
  Button,
  Container,
  FormControl,
  Grid,
  InputLabel,
  MenuItem,
  Paper,
  Select,
  Skeleton,
  Slider,
  Stack,
  TextField,
  Typography,
  Divider,
} from "@mui/material";
import type { SelectChangeEvent } from "@mui/material";
import { useNavigate } from "react-router-dom";
import { http } from "../api/http";

type Props = {
  toggleTheme: () => void;
  themeMode: "light" | "dark";
};

type Subject = { id: number; name: string };

type Tutor = {
  id: string;
  displayName: string;
  bio: string;
  city: string;
  hourlyRate: number;
  minOfferPrice?: number | null;
  subjects: Subject[];
};

type PagedResult<T> = {
  items: T[];
  total: number;
  page: number;
  pageSize: number;
};

type SortKey =
  | "relevance"
  | "priceAsc"
  | "priceDesc"
  | "rateAsc"
  | "rateDesc"
  | "nameAsc";

function clampTextStyles(lines: number) {
  return {
    display: "-webkit-box",
    WebkitLineClamp: lines,
    WebkitBoxOrient: "vertical" as const,
    overflow: "hidden",
  };
}

function formatPrice(pln?: number | null) {
  if (typeof pln !== "number") return null;
  return `${pln} zł`;
}
function avatarUrl(id: string) {
  return `https://api.dicebear.com/7.x/initials/svg?seed=${id}&backgroundColor=edf2f7`;
}


export default function TutorsPage({}: Props) {
  const [tutors, setTutors] = useState<Tutor[]>([]);
  const [loading, setLoading] = useState(true);
  const [loadError, setLoadError] = useState<string | null>(null);

  const [query, setQuery] = useState("");
  const [city, setCity] = useState("");
  const [subjectId, setSubjectId] = useState<number | "">("");
  const [sort, setSort] = useState<SortKey>("relevance");
  const [priceRange, setPriceRange] = useState<[number, number]>([0, 300]);

  const navigate = useNavigate();

  useEffect(() => {
    setLoading(true);
    setLoadError(null);
    http
      .get<PagedResult<Tutor>>("/api/tutors/search")
      .then((res) => setTutors(res.data.items))
      .catch(() => setLoadError("Nie udało się pobrać listy korepetytorów."))
      .finally(() => setLoading(false));
  }, []);

  const cities = useMemo(() => {
    const set = new Set<string>();
    tutors.forEach((t) => {
      const c = (t.city ?? "").trim();
      if (c) set.add(c);
    });
    return Array.from(set).sort((a, b) => a.localeCompare(b, "pl"));
  }, [tutors]);

  const subjects = useMemo(() => {
    const map = new Map<number, string>();
    tutors.forEach((t) => {
      (t.subjects ?? []).forEach((s) => {
        if (!map.has(s.id)) map.set(s.id, s.name);
      });
    });
    return Array.from(map.entries())
      .map(([id, name]) => ({ id, name }))
      .sort((a, b) => a.name.localeCompare(b.name, "pl"));
  }, [tutors]);

  const priceBounds = useMemo(() => {
    const vals: number[] = [];
    tutors.forEach((t) => {
      if (typeof t.minOfferPrice === "number") vals.push(t.minOfferPrice);
    });
    if (vals.length === 0) return { min: 0, max: 300 };
    const min = Math.floor(Math.min(...vals) / 10) * 10;
    const max = Math.ceil(Math.max(...vals) / 10) * 10;
    return { min: Math.max(0, min), max: Math.max(min + 10, max) };
  }, [tutors]);

  useEffect(() => {
    setPriceRange([priceBounds.min, priceBounds.max]);
  }, [priceBounds.min, priceBounds.max]);

  const filtered = useMemo(() => {
    let list = [...tutors];

    const q = query.trim().toLowerCase();
    if (q) {
      list = list.filter((t) =>
        [t.displayName, t.city, t.bio, ...(t.subjects?.map((s) => s.name) ?? [])]
          .join(" ")
          .toLowerCase()
          .includes(q)
      );
    }

    const c = city.trim().toLowerCase();
    if (c) {
      list = list.filter((t) => (t.city ?? "").trim().toLowerCase() === c);
    }

    if (subjectId !== "") {
      list = list.filter((t) => (t.subjects ?? []).some((s) => s.id === subjectId));
    }

    list = list.filter((t) => {
      const p = typeof t.minOfferPrice === "number" ? t.minOfferPrice : null;
      if (p == null) return false;
      return p >= priceRange[0] && p <= priceRange[1];
    });

    const byPrice = (t: Tutor) =>
      typeof t.minOfferPrice === "number"
        ? t.minOfferPrice
        : Number.POSITIVE_INFINITY;

    switch (sort) {
      case "priceAsc":
        list.sort((a, b) => byPrice(a) - byPrice(b));
        break;
      case "priceDesc":
        list.sort((a, b) => byPrice(b) - byPrice(a));
        break;
      case "rateAsc":
        list.sort((a, b) => a.hourlyRate - b.hourlyRate);
        break;
      case "rateDesc":
        list.sort((a, b) => b.hourlyRate - a.hourlyRate);
        break;
      case "nameAsc":
        list.sort((a, b) => a.displayName.localeCompare(b.displayName, "pl"));
        break;
      default:
        break;
    }

    return list;
  }, [tutors, query, city, subjectId, sort, priceRange]);

  const resetFilters = () => {
    setQuery("");
    setCity("");
    setSubjectId("");
    setSort("relevance");
    setPriceRange([priceBounds.min, priceBounds.max]);
  };

  const onSubjectChange = (e: SelectChangeEvent<string>) => {
    const v = e.target.value;
    setSubjectId(v === "" ? "" : Number(v));
  };

  return (
    <Box sx={{ py: 3 }}>
      <Container maxWidth="lg">
        <Grid container spacing={2.5} alignItems="flex-start">
          {/* LEFT: FILTERS */}
          <Grid item xs={12} md={3} lg={2.6}>
            <Paper
              sx={{
                p: 2.5,
                position: { md: "sticky" },
                top: { md: 16 },
                border: "1px solid",
                borderColor: "divider",
                boxShadow: "0 6px 18px rgba(0,0,0,0.04)",
              }}
            >
              <Stack spacing={2}>
                <Box>
                  <Typography fontWeight={1000} sx={{ letterSpacing: -0.2 }}>
                    Filtry
                  </Typography>
                  <Typography variant="body2" color="text.secondary">
                    Dopasuj wyniki do swoich potrzeb
                  </Typography>
                </Box>

                <Divider />

                <TextField
                  label="Szukaj"
                  value={query}
                  onChange={(e) => setQuery(e.target.value)}
                  fullWidth
                />

                <FormControl fullWidth>
                  <InputLabel>Miasto</InputLabel>
                  <Select
                    label="Miasto"
                    value={city}
                    onChange={(e) => setCity(String(e.target.value))}
                  >
                    <MenuItem value="">Wszystkie</MenuItem>
                    {cities.map((c) => (
                      <MenuItem key={c} value={c}>
                        {c}
                      </MenuItem>
                    ))}
                  </Select>
                </FormControl>

                <FormControl fullWidth>
                  <InputLabel>Przedmiot</InputLabel>
                  <Select
                    label="Przedmiot"
                    value={subjectId === "" ? "" : String(subjectId)}
                    onChange={onSubjectChange}
                  >
                    <MenuItem value="">Wszystkie</MenuItem>
                    {subjects.map((s) => (
                      <MenuItem key={s.id} value={String(s.id)}>
                        {s.name}
                      </MenuItem>
                    ))}
                  </Select>
                </FormControl>

                <Box>
                  <Stack direction="row" justifyContent="space-between" alignItems="baseline">
                    <Typography fontWeight={900}>Budżet</Typography>
                    <Typography variant="body2" color="text.secondary">
                      {priceRange[0]}–{priceRange[1]} zł
                    </Typography>
                  </Stack>
                  <Slider
                    value={priceRange}
                    onChange={(_, v) => setPriceRange(v as [number, number])}
                    valueLabelDisplay="auto"
                    min={priceBounds.min}
                    max={priceBounds.max}
                    disableSwap
                  />
                </Box>

                <FormControl fullWidth>
                  <InputLabel>Sortowanie</InputLabel>
                  <Select
                    label="Sortowanie"
                    value={sort}
                    onChange={(e) => setSort(e.target.value as SortKey)}
                  >
                    <MenuItem value="relevance">Domyślne</MenuItem>
                    <MenuItem value="priceAsc">Stawka rosnąco</MenuItem>
                    <MenuItem value="priceDesc">Stawka malejąco</MenuItem>
                    <MenuItem value="nameAsc">Imię A–Z</MenuItem>
                  </Select>
                </FormControl>

                <Button variant="outlined" onClick={resetFilters}>
                  Wyczyść filtry
                </Button>
              </Stack>
            </Paper>
          </Grid>

          {/* RIGHT: RESULTS */}
          <Grid item xs={12} md={9} lg={9.4}>
            <Stack spacing={1.5}>
              {/* Header results bar */}
              <Paper
                sx={{
                  p: 2,
                  border: "1px solid",
                  borderColor: "divider",
                  boxShadow: "0 6px 18px rgba(0,0,0,0.04)",
                }}
              >
                <Stack
                  direction={{ xs: "column", sm: "row" }}
                  spacing={1}
                  justifyContent="space-between"
                  alignItems={{ sm: "center" }}
                >
                  <Box>
                    <Typography fontWeight={1000} sx={{ letterSpacing: -0.2 }}>
                      Oferty korepetycji
                    </Typography>
                    <Typography variant="body2" color="text.secondary">
                      Wybierz korepetytora i przejdź do rezerwacji
                    </Typography>
                  </Box>

                  {!loading && !loadError && (
                    <Typography color="text.secondary">
                      Wyniki: <b>{filtered.length}</b>
                    </Typography>
                  )}
                </Stack>
              </Paper>

              {loadError && <Alert severity="error">{loadError}</Alert>}

              <Grid
                container
                spacing={2}
                sx={{
                  maxHeight: "calc(100vh - 260px)",
                  overflow: "auto",
                  pr: 0.5,
                  pb: 0.5,
                }}
              >
                {loading &&
                  Array.from({ length: 9 }).map((_, i) => (
                    <Grid item xs={12} key={i}>
                      <Paper sx={{ p: 2.25, border: "1px solid", borderColor: "divider" }}>
                        <Stack spacing={1.2}>
                          <Stack direction="row" justifyContent="space-between" alignItems="center">
                            <Skeleton variant="text" width="38%" />
                            <Skeleton variant="rounded" width={120} height={28} />
                          </Stack>
                          <Skeleton variant="text" width="22%" />
                          <Skeleton variant="text" width="92%" />
                          <Skeleton variant="text" width="86%" />
                          <Skeleton variant="rounded" width={140} height={22} />
                        </Stack>
                      </Paper>
                    </Grid>
                  ))}

                {!loading &&
                  !loadError &&
                  filtered.map((t) => {
                    const price = formatPrice(t.minOfferPrice);
                    const subjectPrimary = (t.subjects ?? [])[0]?.name ?? "Przedmiot";
                    const extraSubjects =
                      (t.subjects?.length ?? 0) > 1 ? t.subjects.length - 1 : 0;

                    return (
                      <Grid item xs={12} key={t.id}>
                        <Paper
                          sx={{
                            p: 2.25,
                            cursor: "pointer",
                            border: "1px solid",
                            borderColor: "divider",
                            boxShadow: "0 2px 8px rgba(0,0,0,0.05)",
                            transition:
                              "transform 140ms ease, box-shadow 140ms ease, border-color 140ms ease",
                            "&:hover": {
                              transform: "translateY(-2px)",
                              boxShadow: "0 10px 24px rgba(0,0,0,0.08)",
                              borderColor: "text.secondary",
                            },
                          }}
                          role="button"
                          tabIndex={0}
                          onClick={() => navigate(`/tutors/${t.id}`)}
                          onKeyDown={(e) => {
                            if (e.key === "Enter" || e.key === " ")
                              navigate(`/tutors/${t.id}`);
                          }}
                        >
                          <Stack spacing={1.25}>
                            <Stack
                              direction={{ xs: "column", sm: "row" }}
                              justifyContent="space-between"
                              alignItems={{ sm: "center" }}
                              spacing={1.5}
                            >
                              <Box sx={{ minWidth: 0 }}>
                                <Typography
                                  fontWeight={1000}
                                  sx={{ ...clampTextStyles(1), letterSpacing: -0.2 }}
                                >
                                  {t.displayName}
                                </Typography>
                                <Typography
                                  variant="body2"
                                  color="text.secondary"
                                  sx={{ ...clampTextStyles(1) }}
                                >
                                  {t.city}
                                </Typography>
                              </Box>

                              <Box sx={{ textAlign: { xs: "left", sm: "right" } }}>
                                <Typography
                                  sx={{
                                    fontWeight: 1000,
                                    color: "secondary.main",
                                    fontSize: 18,
                                    lineHeight: 1.1,
                                  }}
                                >
                                  {price ? `Od ${price}` : "Brak ofert"}
                                </Typography>
                                <Typography variant="caption" color="text.secondary">
                                  za lekcję
                                </Typography>
                              </Box>
                            </Stack>

                            <Typography
                              sx={{
                                fontWeight: 900,
                                fontSize: 14,
                                color: "secondary.main",
                                letterSpacing: 0.2,
                              }}
                            >
                              {subjectPrimary}
                              {extraSubjects > 0 ? ` +${extraSubjects}` : ""}
                            </Typography>

                            <Typography sx={{ ...clampTextStyles(3) }} color="text.primary">
                              {t.bio}
                            </Typography>

                            <Button
                              variant="text"
                              size="small"
                              sx={{ alignSelf: "flex-start", px: 0, fontWeight: 900 }}
                              onClick={(e) => {
                                e.stopPropagation();
                                navigate(`/tutors/${t.id}`);
                              }}
                            >
                              Zobacz profil →
                            </Button>
                          </Stack>
                        </Paper>
                      </Grid>
                    );
                  })}

                {!loading && !loadError && filtered.length === 0 && (
                  <Grid item xs={12}>
                    <Paper sx={{ p: 3, border: "1px solid", borderColor: "divider" }}>
                      <Stack spacing={1.5} alignItems="flex-start">
                        <Typography fontWeight={1000}>Brak wyników</Typography>
                        <Typography color="text.secondary">
                          Zmień filtry lub wyszukaj inną frazę.
                        </Typography>
                        <Button variant="outlined" onClick={resetFilters}>
                          Wyczyść filtry
                        </Button>
                      </Stack>
                    </Paper>
                  </Grid>
                )}
              </Grid>
            </Stack>
          </Grid>
        </Grid>
      </Container>
    </Box>
  );
}
