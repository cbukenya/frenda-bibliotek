// ─── Types ────────────────────────────────────────────────────────────────────

export interface Book {
  id: number;
  isbn: string;
  title: string;
  author: string;
  genre: string;
  description: string;
  publishedYear: number;
  totalPages: number;
  coverUrl: string | null;
  totalCopies: number;
  availableCopies: number;
  avgReadingDays: number | null;
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

export const getBooks = (): Promise<Book[]> =>
  apiFetch('/api/books');

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

/** Estimated reading time in hours based on 250wpm average and 250 words/page. */
export function estReadingHours(totalPages: number): string {
  const hours = Math.round((totalPages * 250) / 250 / 60);
  return hours < 1 ? '<1h read' : `${hours}h read`;
}

/** Cover image URL — falls back to a genre-coloured gradient data URI. */
export function coverUrl(book: Pick<Book, 'coverUrl' | 'genre' | 'title'>): string | null {
  return book.coverUrl ?? null;
}

const GENRE_EMOJI: Record<string, string> = {
  'Programming':    '💻',
  'Design':         '🎨',
  'Psychology':     '🧠',
  'History':        '📜',
  'Science Fiction':'🚀',
  'Memoir':         '📖',
  'Self-Help':      '⭐',
  'Philosophy':     '🏛️',
};

export function genreEmoji(genre: string): string {
  return GENRE_EMOJI[genre] ?? '📚';
}
