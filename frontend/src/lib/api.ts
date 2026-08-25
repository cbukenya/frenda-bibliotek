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
  returnedAt: string | null;
}

export interface User {
  id: number;
  name: string;
  email: string;
}

// ─── User context ─────────────────────────────────────────────────────────────

const DEFAULT_USER_ID = 1;

export function getCurrentUserId(): number {
  if (typeof window === 'undefined') return DEFAULT_USER_ID;
  const stored = localStorage.getItem('frenda_user_id');
  return stored ? parseInt(stored, 10) : DEFAULT_USER_ID;
}

export function setCurrentUserId(id: number): void {
  localStorage.setItem('frenda_user_id', String(id));
  window.dispatchEvent(new Event('frenda_user_changed'));
}

// Client-side fetches use relative URLs → Next.js rewrite proxy forwards to the API container.
const API_BASE = '';

async function apiFetch<T>(path: string, options: RequestInit = {}): Promise<T> {
  const userId = getCurrentUserId();
  const res = await fetch(`${API_BASE}${path}`, {
    ...options,
    headers: {
      'Content-Type': 'application/json',
      'X-User-Id': String(userId),
      ...options.headers,
    },
  });

  if (!res.ok) {
    const text = await res.text().catch(() => '');
    throw new Error(`API ${res.status}: ${text || res.statusText}`);
  }

  // 204 No Content
  if (res.status === 204) return undefined as T;
  return res.json();
}

// ─── Books ────────────────────────────────────────────────────────────────────

export const getBooks = (filters?: { genreId?: number; authorId?: number }): Promise<Book[]> => {
  const params = new URLSearchParams();
  if (filters?.genreId) params.set('genreId', String(filters.genreId));
  if (filters?.authorId) params.set('authorId', String(filters.authorId));
  const qs = params.toString();
  return apiFetch(`/api/books${qs ? `?${qs}` : ''}`);
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

// ─── Genres ─────────────────────────────────────────────────────────────────────────────

export const getGenreTree = (): Promise<Genre[]> =>
  apiFetch('/api/genres/tree');

export const getGenreAncestors = (id: number): Promise<Genre[]> =>
  apiFetch(`/api/genres/${id}/ancestors`);

export const getAuthors = (): Promise<Author[]> =>
  apiFetch('/api/authors');

// ─── Helpers ──────────────────────────────────────────────────────────────────

/** Days until due (loans are 14 days). Returns negative if overdue. */
export function daysUntilDue(borrowedAt: string): number {
  const due = new Date(borrowedAt);
  due.setDate(due.getDate() + 14);
  return Math.ceil((due.getTime() - Date.now()) / (1000 * 60 * 60 * 24));
}

export function formatDate(iso: string): string {
  return new Date(iso).toLocaleDateString('en-GB', {
    day: 'numeric', month: 'short', year: 'numeric',
  });
}

/** Estimated reading time from page count (~40 pages/hour). */
export function estReadingTime(totalPages: number): string {
  if (!totalPages || totalPages <= 0) return '';
  const hours = Math.round(totalPages / 40);
  if (hours < 1) return '<1h read';
  return `${hours}h read`;
}

/** Cover image URL — falls back to a genre-coloured gradient data URI. */
export function coverUrl(book: Pick<Book, 'coverUrl' | 'genre' | 'title'>): string | null {
  return book.coverUrl ?? null;
}

const GENRE_EMOJI: Record<string, string> = {
  'Programming':      '💻',
  'Agile & Craftsmanship': '💻',
  'Systems Programming': '⚙️',
  'Design':           '🎨',
  'UX Design':        '🎨',
  'Psychology':       '🧠',
  'Cognitive Psychology': '🧠',
  'History':          '📜',
  'Science Fiction':  '🚀',
  'Space Opera':      '🚀',
  'Cyberpunk':        '🤖',
  'Military Sci-Fi':  '⚔️',
  'Fantasy':          '🧙',
  'Epic Fantasy':     '🧙',
  'Urban Fantasy':    '🏙️',
  'Literary Fiction': '📖',
  'Memoir':           '📖',
  'Self-Help':        '⭐',
  'Philosophy':       '🏛️',
  'Science':          '🔬',
  'Physics':          '⚛️',
  'Technology':       '🖥️',
  'Fiction':          '📚',
  'Non-Fiction':      '📚',
};

export function genreEmoji(genre: string): string {
  return GENRE_EMOJI[genre] ?? '📚';
}
