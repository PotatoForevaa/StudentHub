import { createRoot } from "react-dom/client";
import type { Root } from "react-dom/client";
import { StrictMode } from "react";
import App from "./App";

const container = document.getElementById("root");

if (!container) {
  throw new Error("Root container not found");
}

const root: Root = createRoot(container);

root.render(
  <StrictMode>
    <App />
  </StrictMode>
);
