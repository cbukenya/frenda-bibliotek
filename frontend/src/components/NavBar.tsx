'use client';

import Link from 'next/link';
import { usePathname, useRouter } from 'next/navigation';
import { useEffect, useState } from 'react';
import { useTranslations, useLocale } from 'next-intl';
import { getCurrentUserId, getUser, type User } from '@/lib/api';

export default function NavBar() {
  const t = useTranslations('nav');
  const tc = useTranslations('common');
  const locale = useLocale();
  const pathname = usePathname();
  const router = useRouter();
  const [user, setUser] = useState<User | null>(null);

  useEffect(() => {
    const load = () => getUser(getCurrentUserId()).then(setUser).catch(() => null);
    load();
    window.addEventListener('frenda_user_changed', load);
    return () => window.removeEventListener('frenda_user_changed', load);
  }, []);

  const isActive = (href: string) =>
    href === '/' ? pathname === '/' || pathname === '/sv' : pathname.includes(href);

  const switchLocale = (next: string) => {
    const stripped = pathname.replace(/^\/(en|sv)/, '') || '/';
    router.push(next === 'en' ? stripped : `/${next}${stripped}`);
  };

  const initials = user
    ? user.name.split(' ').map(n => n[0]).join('').slice(0, 2).toUpperCase()
    : '?';

  return (
    <>
      {/* ── Desktop top nav ────────────────────────────────────── */}
      <header className="fixed top-0 w-full z-50 bg-surface/80 border-b border-outline-variant/30 shadow-sm backdrop-blur-md hidden md:block">
        <div className="flex justify-between items-center h-16 px-margin-desktop max-w-container-max mx-auto">
          <div className="flex items-center gap-8">
            <Link
              href="/"
              className="font-headline-md text-headline-md font-bold text-primary"
            >
              {t('logo')}
            </Link>

            <nav className="flex gap-6 items-center">
              {([
                { href: '/',         label: t('browse')   },
                { href: '/loans',    label: t('myLoans')  },
                { href: '/discover', label: t('discover') },
              ] as const).map(({ href, label }) => (
                <Link
                  key={href}
                  href={href}
                  className={`pb-1 font-label-md text-label-md transition-all duration-150 ease-in-out ${
                    isActive(href)
                      ? 'text-primary border-primary border-b-2'
                      : 'text-on-surface-variant'
                  }`}
                >
                  {label}
                </Link>
              ))}
            </nav>
          </div>

          <div className="flex items-center gap-4">
            {/* Search */}
            <div className="relative">
              <input
                className="bg-[#F1F3F5] border-none rounded-full py-2 pl-10 pr-4 text-sm focus:ring-2 focus:ring-primary focus:bg-white transition-all w-64 outline-none"
                placeholder={t('searchPlaceholder')}
                type="search"
                onKeyDown={e => {
                  if (e.key === 'Enter') {
                    const v = (e.target as HTMLInputElement).value.trim();
                    if (v) router.push(`/?q=${encodeURIComponent(v)}`);
                  }
                }}
              />
              <span className="material-symbols-outlined absolute left-3 top-2.5 text-on-surface-variant text-[18px]">
                search
              </span>
            </div>

            {/* Locale switcher */}
            <div className="flex gap-1">
              {(['en', 'sv'] as const).map(lang => (
                <button
                  key={lang}
                  onClick={() => switchLocale(lang)}
                  className={`text-xs font-bold px-2 py-1 rounded transition-colors ${
                    locale === lang
                      ? 'bg-primary text-on-primary'
                      : 'text-on-surface-variant hover:bg-surface-container-low'
                  }`}
                >
                  {tc(lang === 'en' ? 'english' : 'swedish')}
                </button>
              ))}
            </div>

            {/* Notifications */}
            <button className="p-2 text-on-surface-variant hover:bg-surface-container-low rounded-full transition-colors relative">
              <span className="material-symbols-outlined">notifications</span>
              <span className="absolute top-2 right-2 w-2 h-2 bg-error rounded-full" />
            </button>

            {/* Avatar */}
            <div
              className="w-8 h-8 rounded-full bg-primary-container text-on-primary flex items-center justify-center text-xs font-bold border border-outline-variant cursor-pointer"
              title={user?.name}
            >
              {initials}
            </div>
          </div>
        </div>
      </header>

      {/* ── Mobile bottom nav ──────────────────────────────────── */}
      <nav className="fixed bottom-0 left-0 w-full z-50 flex justify-around items-center px-4 h-16 md:hidden bg-surface border-t border-outline-variant/30 shadow-[0_-4px_20px_rgba(71,80,144,0.08)]">
        {([
          { href: '/',         icon: 'library_books', label: t('browse')   },
          { href: '/loans',    icon: 'book_4',        label: t('myLoans')  },
          { href: '/discover', icon: 'explore',       label: t('discover') },
        ] as const).map(({ href, icon, label }) => {
          const active = isActive(href);
          return (
            <Link
              key={href}
              href={href}
              className={`flex flex-col items-center justify-center rounded-xl px-4 py-1 transition-all ${
                active ? 'bg-primary-container text-on-primary' : 'text-on-surface-variant'
              }`}
            >
              <span className={`material-symbols-outlined text-xl mb-0.5 ${active ? 'filled' : ''}`}>
                {icon}
              </span>
              <span className="font-label-sm text-[10px]">{label}</span>
            </Link>
          );
        })}
      </nav>
    </>
  );
}
