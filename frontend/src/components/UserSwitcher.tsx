'use client';

import { useEffect, useState } from 'react';
import { useTranslations } from 'next-intl';
import { getCurrentUserId, setCurrentUserId, getUsers, type User } from '@/lib/api';

export default function UserSwitcher() {
  const t = useTranslations('dev');
  const [users, setUsers] = useState<User[]>([]);
  const [currentId, setCurrentId] = useState(getCurrentUserId());

  useEffect(() => {
    getUsers().then(setUsers).catch(() => []);
  }, []);

  if (users.length === 0) return null;

  const handleChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    const id = parseInt(e.target.value, 10);
    setCurrentId(id);
    setCurrentUserId(id);
  };

  return (
    <div className="fixed bottom-16 md:bottom-0 left-0 right-0 z-40 flex items-center justify-center gap-3 bg-surface-container-low/90 backdrop-blur-sm border-t border-outline-variant/30 py-2 px-4 font-body-sm text-body-sm text-on-surface-variant">
      <span>{t('label')}</span>
      <select
        value={currentId}
        onChange={handleChange}
        className="bg-surface-container rounded px-2 py-1 text-sm border border-outline-variant/50 focus:ring-1 focus:ring-primary outline-none"
      >
        {users.map(u => (
          <option key={u.id} value={u.id}>{u.name}</option>
        ))}
      </select>
    </div>
  );
}
