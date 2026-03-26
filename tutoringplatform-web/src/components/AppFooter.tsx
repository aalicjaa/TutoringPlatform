import { Box, Container, Stack, Typography, Link } from "@mui/material";
import { Link as RouterLink } from "react-router-dom";

export default function AppFooter() {
  const year = new Date().getFullYear();

  return (
    <Box
      component="footer"
      sx={{
        py: 2.5,
        background: "linear-gradient(90deg, #1e3a5f 0%, #2b4f78 100%)",
        boxShadow: "0 -2px 8px rgba(0,0,0,0.05)",
        color: "primary.contrastText",
      }}
    >
      <Container maxWidth="lg">
        <Stack
          direction={{ xs: "column", sm: "row" }}
          justifyContent="space-between"
          alignItems={{ xs: "center", sm: "center" }}
          spacing={1}
        >
          <Typography fontWeight={600}>© {year} EduMatch</Typography>

          <Stack direction="row" spacing={2}>
            <Link component={RouterLink} to="/privacy" color="inherit" underline="hover">
              Polityka prywatności
            </Link>
            <Link component={RouterLink} to="/terms" color="inherit" underline="hover">
              Regulamin
            </Link>
            <Link component={RouterLink} to="/contact" color="inherit" underline="hover">
              Kontakt
            </Link>
          </Stack>
        </Stack>
      </Container>
    </Box>
  );
}
