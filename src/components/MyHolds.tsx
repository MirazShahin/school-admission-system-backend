import { useEffect, useState } from 'react';
import { api, ApiError } from '../api';
import type { HoldSummary } from '../types';

export default function MyHolds({ studentId }: { studentId: string }) {
  const [holds, setHolds] = useState<HoldSummary[]>([]);
  const [error, setError] = useState('');
  const [now, setNow] = useState(Date.now());

  // Refresh the list and the countdown every second.
  useEffect(() => {
    if (!studentId) return;
    let alive = true;
    const load = () =>
      api<HoldSummary[]>('/me/holds', { studentId })
        .then((rows) => alive && (setHolds(rows), setError('')))
        .catch((e) => alive && setError(e instanceof Error ? e.message : 'Could not load.'));
    load();
    const poll = setInterval(load, 3000);
    const tick = setInterval(() => setNow(Date.now()), 1000);
    return () => {
      alive = false;
      clearInterval(poll);
      clearInterval(tick);
    };
  }, [studentId]);

  async function act(holdId: string, action: 'confirm' | 'cancel') {
    setError('');
    try {
      if (action === 'confirm') {
        await api(`/holds/${holdId}/confirm`, { method: 'POST', studentId });
      } else {
        await api(`/holds/${holdId}`, { method: 'DELETE', studentId });
      }
      const rows = await api<HoldSummary[]>('/me/holds', { studentId });
      setHolds(rows);
    } catch (e) {
      setError(e instanceof ApiError ? e.message : 'Action failed.');
    }
  }

  if (!studentId) return <p style={{ padding: 16 }}>Enter your Student ID above.</p>;

  return (
    <section style={{ padding: 16 }}>
      <h2>My applications</h2>
      {error && <p style={{ color: 'crimson' }}>{error}</p>}
      {holds.length === 0 && <p>No applications yet.</p>}
      <ul style={{ listStyle: 'none', padding: 0 }}>
        {holds.map((h) => {
          const secondsLeft = Math.max(0, Math.floor((new Date(h.expiresAt).getTime() - now) / 1000));
          const expired = h.status === 'EXPIRED' || (h.status === 'HELD' && secondsLeft === 0);
          return (
            <li key={h.holdId} style={{ padding: 12, border: '1px solid #ddd', marginBottom: 8 }}>
              <strong>{h.className}</strong> — {expired ? 'EXPIRED' : h.status}
              {h.status === 'HELD' && !expired && (
                <span style={{ marginLeft: 12 }}>
                  {Math.floor(secondsLeft / 60)}:{String(secondsLeft % 60).padStart(2, '0')} left
                </span>
              )}
              <div style={{ marginTop: 8 }}>
                <button
                  disabled={expired || h.status !== 'HELD'}
                  onClick={() => act(h.holdId, 'confirm')}
                >
                  ভর্তি ফি দিন
                </button>{' '}
                <button
                  disabled={expired || h.status !== 'HELD'}
                  onClick={() => act(h.holdId, 'cancel')}
                >
                  বাতিল
                </button>
              </div>
            </li>
          );
        })}
      </ul>
    </section>
  );
}