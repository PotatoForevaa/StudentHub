import type { User } from "../types";

export const hasRole = (user: User | null | undefined, role: string) =>
  Boolean(user?.roles?.includes(role));

export const isAdmin = (user: User | null | undefined) => hasRole(user, "Admin");

export const isModerator = (user: User | null | undefined) =>
  hasRole(user, "Moderator");

export const canModerate = (user: User | null | undefined) =>
  isAdmin(user) || isModerator(user);
