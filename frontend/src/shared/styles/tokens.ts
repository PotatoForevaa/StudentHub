export const colors = {
  // Primary palette
  primary: '#2563eb',       // blue-600
  primaryDark: '#1d4ed8',   // blue-700
  primaryLight: '#3b82f6',  // blue-500

  // Neutral palette
  white: '#ffffff',
  bg: '#f4f6fb',            // page background
  surface: '#ffffff',       // cards, panels

  // Gray palette
  gray100: '#f3f4f6',
  gray300: '#d1d5db',
  gray400: '#9ca3af',
  gray500: '#6b7280',

  // Text colors
  textPrimary: '#0f172a',   // dark text
  textSecondary: '#374151', // secondary text
  muted: '#6b7280',         // tertiary / muted text
  placeholder: 'rgba(15,23,42,0.36)',

  // Accents & utility
  accentBorder: 'rgba(37,99,235,0.08)',
  accentBorderLight: 'rgba(2,6,23,0.04)',
  accentBorderDark: 'rgba(2,6,23,0.06)',
};

export const shadows = {
  sm: '0 8px 24px rgba(15,23,42,0.08)',
  md: '0 20px 40px rgba(0,0,0,0.55)',
  lg: '0 28px 48px rgba(2,6,23,0.12)',
  header: '0 6px 20px rgba(2,6,23,0.06)',
};

export const spacing = {
  xs: '4px',
  sm: '8px',
  md: '12px',
  lg: '16px',
  xl: '20px',
  xxl: '28px',
  xxxl: '36px',
};

export const borderRadius = {
  sm: '8px',
  md: '10px',
  lg: '12px',
  xl: '14px',
  xxl: '16px',
  full: '9999px',
};

export const transitions = {
  fast: '0.15s ease',
  base: '0.18s ease',
  slow: '0.22s ease',
};

export const zIndex = {
  header: 1000,
  modal: 1100,
  dropdown: 950,
};

export const fonts = {
  family: "Inter, ui-sans-serif, system-ui, -apple-system, 'Segoe UI', Roboto, 'Helvetica Neue', Arial",
  size: {
    xs: '12px',
    sm: '13px',
    base: '15px',
    lg: '16px',
    xl: '18px',
    '2xl': '20px',
    '3xl': '22px',
    '4xl': '24px',
  },
  weight: {
    normal: 400,
    medium: 500,
    semibold: 600,
    bold: 700,
  },
};
