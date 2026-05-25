export const colors = {
  // primary palette - modern vibrant blue
  primary: '#3b82f6',       // vibrant blue-500
  primaryDark: '#2563eb',  // blue-600
  primaryLight: '#60a5fa', // blue-400
  primaryLighter: '#dbeafe', // blue-100 for backgrounds

  // secondary palette - elegant violet
  secondary: '#8b5cf6',    // violet-500
  secondaryDark: '#7c3aed', // violet-600
  secondaryLight: '#a78bfa', // violet-400

  // white palette
  white: '#ffffff',
  bg: '#f8fafc',            // page background - subtle cool gray
  surface: '#ffffff',       // cards, panels

  // gray palette - refined
  gray100: '#f1f5f9',
  gray200: '#e2e8f0',
  gray300: '#cbd5e1',
  gray400: '#94a3b8',
  gray500: '#64748b',
  gray600: '#475569',

  // text - improved contrast
  textPrimary: '#0f172a',   // slate-900
  textSecondary: '#334155', // slate-700
  muted: '#64748b',         // slate-500
  placeholder: 'rgba(15,23,42,0.4)',

  // accents
  accentBorder: 'rgba(59,130,246,0.12)',
  accentBorderLight: 'rgba(15,23,42,0.06)',
  accentBorderDark: 'rgba(15,23,42,0.1)',

  // semantic colors
  success: '#10b981',       // emerald-500
  successLight: '#d1fae5', // emerald-100
  warning: '#f59e0b',       // amber-500
  warningLight: '#fef3c7', // amber-100
  error: '#ef4444',         // red-500
  errorLight: '#fee2e2',    // red-100

  // gradients
  gradientPrimary: 'linear-gradient(135deg, #3b82f6 0%, #8b5cf6 100%)',
  gradientBg: 'linear-gradient(180deg, #f8fafc 0%, #f1f5f9 100%)',
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
