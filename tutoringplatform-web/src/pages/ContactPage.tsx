import { Box, Container, Paper, Stack, Typography, Link, Divider } from "@mui/material";
import EmailOutlinedIcon from "@mui/icons-material/EmailOutlined";
import LocationOnOutlinedIcon from "@mui/icons-material/LocationOnOutlined";
import SchoolOutlinedIcon from "@mui/icons-material/SchoolOutlined";
import DescriptionOutlinedIcon from "@mui/icons-material/DescriptionOutlined";
import { Link as RouterLink } from "react-router-dom";

function Item({
  icon,
  label,
  value,
  action,
}: {
  icon: React.ReactNode;
  label: string;
  value: React.ReactNode;
  action?: React.ReactNode;
}) {
  return (
    <Stack direction="row" spacing={1.5} alignItems="center">
      <Box sx={{ color: "primary.main", display: "flex", alignItems: "center" }}>{icon}</Box>

      <Box sx={{ flex: 1, minWidth: 0 }}>
        <Typography variant="body2" color="text.secondary" sx={{ lineHeight: 1.2 }}>
          {label}
        </Typography>
        <Typography sx={{ fontWeight: 900, wordBreak: "break-word" }}>{value}</Typography>
      </Box>

      {action}
    </Stack>
  );
}

export default function ContactPage() {
  return (
    <Box sx={{ py: 4 }}>
      <Container maxWidth="sm">
        <Paper
          sx={{
            p: { xs: 2.5, sm: 3 },
            borderRadius: 3,
            border: "1px solid",
            borderColor: "divider",
            boxShadow: "0 6px 18px rgba(0,0,0,0.05)",
          }}
        >
          <Stack spacing={2.25}>
            <Stack spacing={0.6}>
              <Typography variant="h4" fontWeight={1000} sx={{ letterSpacing: -0.4 }}>
                Kontakt
              </Typography>
              <Typography color="text.secondary">
                Dane kontaktowe i dokumenty związane z aplikacją EduMatch.
              </Typography>
            </Stack>

            <Divider />

            <Stack spacing={1.6}>
              <Item
                icon={<EmailOutlinedIcon />}
                label="E-mail"
                value={
                  <Link
                    href="mailto:amolek@student.wsb-nlu.edu.pl"
                    underline="hover"
                    sx={{ fontWeight: 900, color: "primary.main" }}
                  >
                    amolek@student.wsb-nlu.edu.pl
                  </Link>
                }
              />

              <Item
                icon={<LocationOnOutlinedIcon />}
                label="Adres"
                value="Osiedle Bohaterów Września, Kraków"
              />

              <Item
                icon={<SchoolOutlinedIcon />}
                label="Projekt"
                value="Projekt inżynierski WSB-NLU - EduMatch"
              />

              <Item
                icon={<DescriptionOutlinedIcon />}
                label="Dokumenty"
                value={
                  <Stack direction={{ xs: "column", sm: "row" }} spacing={{ xs: 0.4, sm: 2 }}>
                    <Link
                      component={RouterLink}
                      to="/privacy"
                      underline="hover"
                      sx={{ fontWeight: 900, color: "secondary.main" }}
                    >
                      Polityka prywatności
                    </Link>
                    <Link
                      component={RouterLink}
                      to="/terms"
                      underline="hover"
                      sx={{ fontWeight: 900, color: "secondary.main" }}
                    >
                      Regulamin
                    </Link>
                  </Stack>
                }
              />
            </Stack>
          </Stack>
        </Paper>
      </Container>
    </Box>
  );
}
