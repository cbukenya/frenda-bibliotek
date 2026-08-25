'use client';

import Link from 'next/link';
import { usePathname } from 'next/navigation';
import { useEffect, useState } from 'react';
import { getCurrentUserId, getUser, type User } from '@/lib/api';

export default function NavBar() {
  const pathname = usePathname();
  const [user, setUser] = useState<User | null>(null);
  const [query, setQuery] = useState('');

  useEffect(() => {
    const load = () => {
      const id = getCurrentUserId();
      getUser(id).then(setUser).catch(() => null);
    };
    load();
    window.addEventListener('frenda_user_changed', load);
    return () => window.removeEventListener('frenda_user_changed', load);
  }, []);

  const initials = user
    ? user.name.split(' ').map(n => n[0]).join('').slice(0, 2).toUpperCase()
    : '?';

  const isActive = (href: string) =>
    href === '/' ? pathname === '/' : pathname.startsWith(href);

  return (
    <nav className="nav">
      <Link href="/" className="nav-logo">Frenda Bibliotek</Link>

      <ul className="nav-links">
        {[
          { href: '/',         label: 'Browse' },
          { href: '/loans',    label: 'My Loans' },
          { href: '/discover', label: 'Discover' },
        ].map(({ href, label }) => (
          <li key={href}>
            <Link href={href} className={isActive(href) ? 'active' : ''}>
              {label}
            </Link>
          </li>
        ))}
      </ul>

      <div className="nav-search">
        <svg className="nav-search-icon" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5">
          <circle cx="11" cy="11" r="8"/><path d="m21 21-4.35-4.35"/>
        </svg>
        <input
          type="search"
          placeholder="Search library..."
          value={query}
          onChange={e => setQuery(e.target.value)}
          onKeyDown={e => {
            if (e.key === 'Enter' && query.trim()) {
              window.location.href = `/?q=${encodeURIComponent(query.trim())}`;
            }
          }}
        />
      </div>

      <div className="nav-right">
        <button className="nav-icon-btn" aria-label="Notifications">
          <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
            <path d="M18 8A6 6 0 0 0 6 8c0 7-3 9-3 9h18s-3-2-3-9"/>
            <path d="M13.73 21a2 2 0 0 1-3.46 0"/>
          </svg>
        </button>
        <div className="nav-avatar" title={user?.name ?? 'User'}>{initials}</div>
      </div>
    </nav>
  );
}
