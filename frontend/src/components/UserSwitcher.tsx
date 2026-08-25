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

  const handleChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    const id = parseInt(e.target.value, 10);
    setCurrentId(id);
    setCurrentUserId(id);
  };

  if (users.length === 0) return null;
  const current = users.find(u => u.id === currentId);

  return (
    <div className="dev-footer">
      <span>{t('label')}</span>
      <select value={currentId} onChange={handleChange}>
        {users.map(u => (
          <option key={u.id} value={u.id}>{u.name} ({u.email})</option>
        ))}
      </select>
      {current && <span style={{ color: '#adb5bd' }}>ID: {current.id}</span>}
    </div>
  );
}
