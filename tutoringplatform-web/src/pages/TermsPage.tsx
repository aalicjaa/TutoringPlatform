import { Box, Container, Divider, Paper, Stack, Typography } from "@mui/material";

function BlockTitle({ children }: { children: React.ReactNode }) {
  return (
    <Stack spacing={0.6}>
      <Typography variant="h6" fontWeight={1000} sx={{ letterSpacing: -0.2 }}>
        {children}
      </Typography>
      <Box sx={{ height: 3, width: 56, bgcolor: "secondary.main" }} />
    </Stack>
  );
}

function Row({ title, text }: { title: string; text: string }) {
  return (
    <Stack spacing={0.35}>
      <Typography fontWeight={900} sx={{ color: "primary.main" }}>
        {title}
      </Typography>
      <Typography color="text.secondary">{text}</Typography>
    </Stack>
  );
}

export default function TermsPage() {
  return (
    <Box sx={{ py: 3 }}>
      <Container maxWidth="md">
        <Stack spacing={2}>
          <Paper
            sx={{
              p: { xs: 2.25, sm: 3 },
              borderRadius: 1.5,
              border: "1px solid",
              borderColor: "divider",
              boxShadow: "0 10px 28px rgba(0,0,0,0.06)",
            }}
          >
            <Stack spacing={1}>
              <Typography
                variant="h4"
                fontWeight={1000}
                sx={{ letterSpacing: -0.4, color: "primary.main" }}
              >
                Regulamin
              </Typography>
              <Typography color="text.secondary">
                Zasady korzystania z aplikacji EduMatch i podstawowe reguły rezerwacji zajęć.
              </Typography>

              <Box
                sx={{
                  mt: 1,
                  p: 1.5,
                  borderRadius: 1.5,
                  border: "1px solid rgba(242, 159, 5, 0.28)",
                  bgcolor: "rgba(242, 159, 5, 0.10)",
                }}
              >
                <Typography fontWeight={900} sx={{ color: "secondary.dark" }}>
                  Najważniejsze zasady
                </Typography>
                <Typography variant="body2" color="text.secondary" sx={{ mt: 0.4 }}>
                  Rezerwacje tworzy uczeń, dostępność ustala tutor. Nieopłacone rezerwacje można anulować.
                </Typography>
              </Box>
            </Stack>
          </Paper>

          <Paper
            sx={{
              p: { xs: 2.25, sm: 3 },
              borderRadius: 1.5,
              border: "1px solid",
              borderColor: "divider",
              boxShadow: "0 10px 28px rgba(0,0,0,0.06)",
            }}
          >
            <Stack spacing={2}>
              <BlockTitle>Warunki korzystania</BlockTitle>

              <Row
                title="Konto"
                text="Korzystanie z aplikacji wymaga konta. Użytkownik odpowiada za bezpieczeństwo danych logowania."
              />

              <Divider />

              <Row
                title="Profil tutora i oferty"
                text="Tutor uzupełnia profil oraz dodaje oferty (przedmiot, czas, cena, tryb) i dostępność terminów."
              />

              <Divider />

              <Row
                title="Rezerwacje"
                text="Uczeń może zarezerwować dostępny termin. Status rezerwacji widoczny jest w zakładce Moje rezerwacje."
              />

              <Divider />

              <Row
                title="Płatności"
                text="Opłacenie rezerwacji zmienia jej status na Opłacona. Po opłaceniu anulowanie przez ucznia jest niedostępne."
              />

              <Divider />

              <Row
                title="Anulowanie i odwołanie"
                text="Uczeń może anulować wyłącznie nieopłaconą rezerwację. Tutor może odwołać zajęcia w uzasadnionych sytuacjach."
              />

              <Divider />

              <Row
                title="Odpowiedzialność"
                text="Aplikacja zapewnia narzędzia do rezerwacji i zarządzania ofertami; przebieg zajęć zależy od ustaleń stron."
              />
            </Stack>
          </Paper>
        </Stack>
      </Container>
    </Box>
  );
}
