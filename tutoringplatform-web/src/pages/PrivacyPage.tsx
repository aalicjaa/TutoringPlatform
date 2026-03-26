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

export default function PrivacyPage() {
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
                Polityka prywatności
              </Typography>
             

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
              <BlockTitle>Najważniejsze informacje</BlockTitle>

              <Row
                title="Administrator"
                text="Administratorem danych jest właściciel aplikacji EduMatch (dane w zakładce Kontakt)."
              />

              <Divider />

              <Row
                title="Zakres danych"
                text="Przetwarzamy m.in. e-mail, dane profilu tutora (nazwa, miasto, opis, stawka), informacje o rezerwacjach oraz dane techniczne niezbędne do działania."
              />

              <Divider />

              <Row
                title="Cele"
                text="Logowanie i bezpieczeństwo kont, obsługa ofert tutora, dostępności i rezerwacji oraz utrzymanie poprawnego działania aplikacji."
              />

              <Divider />

              <Row
                title="Udostępnianie"
                text="Dane mogą być przekazywane wyłącznie podmiotom technicznym obsługującym działanie systemu lub podmiotom uprawnionym przepisami prawa."
              />

              <Divider />

              <Row
                title="Prawa użytkownika"
                text="Masz prawo dostępu do danych, ich sprostowania oraz usunięcia w uzasadnionych przypadkach."
              />

              <Divider />

              <Row
                title="Pliki przeglądarki"
                text="Aplikacja korzysta z localStorage/sessionStorage do przechowywania ustawień (motyw) i sesji logowania."
              />
            </Stack>
          </Paper>
        </Stack>
      </Container>
    </Box>
  );
}
