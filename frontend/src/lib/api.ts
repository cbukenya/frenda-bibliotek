// ─── Types ────────────────────────────────────────────────────────────────────

export interface Book {
  id: number;
  isbn: string;
  title: string;
  author: string;
  authorId: number;
  genre: string;
  genreId: number;
  description: string;
  publishedYear: number;
  totalPages: number;
  coverUrl: string | null;
  totalCopies: number;
  availableCopies: number;
  avgReadingDays: number | null;
}

export interface Genre {
  id: number;
  name: string;
  slug: string;
  parentId: number | null;
  children?: Genre[];
}

export interface Author {
  id: number;
  name: string;
  slug: string;
}

export interface BookDetail extends Book {
  recommendations: Book[];
}

export interface Loan {
  id: number;
  bookId: number;
  isbn: string;
  bookTitle: string;
  bookAuthor: string;
  coverUrl: string | null;
  borrowedAt: string;
  dueDate: string;        // set server-side as borrowedAt + 14 days
  returnedAt: string | null;
}

export interface User {
  id: number;
  name: string;
  email: string;
}

export interface AuthResponse {
  token: string;
  user: User;
}

// ─── Auth ─────────────────────────────────────────────────────────────────────

const TOKEN_KEY = 'frenda_token';
const USER_KEY = 'frenda_user';

export function getToken(): string | null {
  if (typeof window === 'undefined') return null;
  return localStorage.getItem(TOKEN_KEY);
}

export function getStoredUser(): User | null {
  if (typeof window === 'undefined') return null;
  const raw = localStorage.getItem(USER_KEY);
  if (!raw) return null;
  try { return JSON.parse(raw); } catch { return null; }
}

export function isLoggedIn(): boolean {
  return !!getToken();
}

function setAuth(token: string, user: User): void {
  localStorage.setItem(TOKEN_KEY, token);
  localStorage.setItem(USER_KEY, JSON.stringify(user));
  // Set cookie for middleware route protection
  document.cookie = `frenda_token=${token}; path=/; max-age=${60 * 60 * 24 * 7}; SameSite=Lax`;
  window.dispatchEvent(new Event('frenda_user_changed'));
}

export function logout(): void {
  localStorage.removeItem(TOKEN_KEY);
  localStorage.removeItem(USER_KEY);
  document.cookie = 'frenda_token=; path=/; max-age=0';
  window.location.href = '/login';
}

// ─── API Client ───────────────────────────────────────────────────────────────

const API_BASE = '';

async function apiFetch<T>(path: string, options: RequestInit = {}): Promise<T> {
  const token = getToken();
  const headers: Record<string, string> = {
    'Content-Type': 'application/json',
    ...options.headers as Record<string, string>,
  };
  if (token) {
    headers['Authorization'] = `Bearer ${token}`;
  }

  const res = await fetch(`${API_BASE}${path}`, {
    ...options,
    headers,
  });

  // If unauthorized and user had a token (expired), clear and redirect
  if (res.status === 401) {
    if (typeof window !== 'undefined' && token) {
      logout();
    }
    throw new Error('Unauthorized');
  }

  if (!res.ok) {
    const text = await res.text().catch(() => '');
    throw new Error(`API ${res.status}: ${text || res.statusText}`);
  }

  if (res.status === 204) return undefined as T;
  return res.json();
}

// ─── Auth endpoints (no token needed) ─────────────────────────────────────────

export async function login(email: string, password: string): Promise<AuthResponse> {
  const res = await fetch(`${API_BASE}/api/auth/login`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ email, password }),
  });

  if (!res.ok) {
    const data = await res.json().catch(() => ({ error: res.statusText }));
    throw new Error(data.error || 'Login failed');
  }

  const data: AuthResponse = await res.json();
  setAuth(data.token, data.user);
  return data;
}

export async function register(name: string, email: string, password: string): Promise<AuthResponse> {
  const res = await fetch(`${API_BASE}/api/auth/register`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ name, email, password }),
  });

  if (!res.ok) {
    const data = await res.json().catch(() => ({ error: res.statusText }));
    throw new Error(data.error || 'Registration failed');
  }

  const data: AuthResponse = await res.json();
  setAuth(data.token, data.user);
  return data;
}

// ─── Books ────────────────────────────────────────────────────────────────────

export const getBooks = async (filters?: { genreId?: number; authorId?: number }): Promise<Book[]> => {
  const params = new URLSearchParams();
  if (filters?.genreId) params.set('genreId', String(filters.genreId));
  if (filters?.authorId) params.set('authorId', String(filters.authorId));
  // Request all books in one page to keep existing UI unchanged
  params.set('pageSize', '1000');
  const qs = params.toString();
  const result = await apiFetch(`/api/books?${qs}`) as { items: Book[] };
  // Backend now returns { items, total, page, pageSize } — unwrap for callers
  return result.items;
};

export interface PagedBooks {
  items: Book[];
  total: number;
  page: number;
  pageSize: number;
}

export const getBooksPaged = async (
  page: number,
  pageSize: number,
  filters?: { genreId?: number; authorId?: number }
): Promise<PagedBooks> => {
  const params = new URLSearchParams();
  params.set('page', String(page));
  params.set('pageSize', String(pageSize));
  if (filters?.genreId) params.set('genreId', String(filters.genreId));
  if (filters?.authorId) params.set('authorId', String(filters.authorId));
  return await apiFetch(`/api/books?${params.toString()}`) as PagedBooks;
};

export const getTopBooks = (): Promise<Book[]> =>
  apiFetch('/api/books/top');

export const getBook = (id: number): Promise<BookDetail> =>
  apiFetch(`/api/books/${id}`);

// ─── Loans ────────────────────────────────────────────────────────────────────

export const getMyLoans = (): Promise<Loan[]> =>
  apiFetch('/api/loans');

export const borrowBook = (isbn: string): Promise<Loan> =>
  apiFetch('/api/loans', {
    method: 'POST',
    body: JSON.stringify({ isbn }),
  });

export const returnLoan = (loanId: number): Promise<Loan> =>
  apiFetch(`/api/loans/${loanId}/return`, { method: 'PATCH' });

// ─── Users ────────────────────────────────────────────────────────────────────

export const getUsers = (): Promise<User[]> =>
  apiFetch('/api/users');

export const getUser = (id: number): Promise<User> =>
  apiFetch(`/api/users/${id}`);

// ─── Genres ───────────────────────────────────────────────────────────────────

export const getGenreTree = (): Promise<Genre[]> =>
  apiFetch('/api/genres/tree');

export const getGenreAncestors = (id: number): Promise<Genre[]> =>
  apiFetch(`/api/genres/${id}/ancestors`);

export const getAuthors = (): Promise<Author[]> =>
  apiFetch('/api/authors');

// ─── Helpers ──────────────────────────────────────────────────────────────────

export function daysUntilDue(loan: Loan): number {
  // Use the server-supplied dueDate rather than assuming a fixed 14-day window
  const due = new Date(loan.dueDate);
  return Math.ceil((due.getTime() - Date.now()) / (1000 * 60 * 60 * 24));
}

export function formatDate(iso: string): string {
  return new Date(iso).toLocaleDateString('en-GB', {
    day: 'numeric', month: 'short', year: 'numeric',
  });
}

export function estReadingTime(totalPages: number): string {
  if (!totalPages || totalPages <= 0) return '';
  const hours = Math.round(totalPages / 40);
  if (hours < 1) return '<1h read';
  return `${hours}h read`;
}

export function coverUrl(book: Pick<Book, 'coverUrl' | 'genre' | 'title'>): string | null {
  return book.coverUrl ?? null;
}

const GENRE_EMOJI: Record<string, string> = {
  'Programming':      '💻',
  'Agile & Craftsmanship': '💻',
  'Design':           '🎨',
  'UX Design':        '🎨',
  'Psychology':       '🧠',
  'History':          '📜',
  'Science Fiction':  '🚀',
  'Space Opera':      '🚀',
  'Fantasy':          '🧙',
  'Memoir':           '📖',
  'Self-Help':        '⭐',
  'Philosophy':       '🏛️',
  'Fiction':          '📚',
  'Non-Fiction':      '📚',
};

export function genreEmoji(genre: string): string {
  return GENRE_EMOJI[genre] ?? '📚';
}
