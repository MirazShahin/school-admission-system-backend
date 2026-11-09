import { useEffect, useState } from 'react';
import { api, ApiError } from '../api';
import type { ClassItem } from '../types';

export default function ClassList({ studentId }: { studentId: string }) {
  const [classes, setClasses] = useState<ClassItem[]>([]);
  const [error, setError] = useState('');
  const [busy, setBusy] = useState<string | null>(null);

  // Poll every 3 seconds so seat counts update without reloading.
  useEffect(() => {
    let alive = true;
    const load = () =>
      api<ClassItem[]>('/classes')
        .then((rows) => alive && (setClasses(rows), setError('')))
        .catch((e) => alive && setError(e instanceof Error ? e.message : 'Could not load classes.'));
    load();
    const timer = setInterval(load, 3000);
    return () => {
      alive = false;
      clearInterval(timer);
    };
  }, []);

  async function apply(classId: string) {
    setBusy(classId);
    setError('');
    try {
      await api(`/classes/${classId}/holds`, { method: 'POST', studentId });
      alert('Applied. Confirm it from My applications before the hold expires.');
    } catch (e) {
      setError(e instanceof ApiError ? e.message : 'Could not apply.');
    } finally {
      setBusy(null);
    }
  }

  return (
    <section style={{ padding: 16 }}>
      <h2>Classes</h2>
      {error && <p style={{ color: 'crimson' }}>{error}</p>}
      <table cellPadding={8} style={{ borderCollapse: 'collapse' }}>
        <thead>
          <tr>
            <th align="left">Name</th>
            <th>Total</th>
            <th>Held</th>
            <th>Confirmed</th>
            <th>Available</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          {classes.map((c) => (
            <tr key={c.id}>
              <td>{c.name}</td>
              <td align="center">{c.totalSeats}</td>
              <td align="center">{c.heldSeats}</td>
              <td align="center">{c.confirmedSeats}</td>
              <td align="center">{c.availableSeats}</td>
              <td>
                <button
                  disabled={!studentId || c.availableSeats === 0 || busy === c.id}
                  onClick={() => apply(c.id)}
                >
                  {c.availableSeats === 0 ? 'Full' : 'Apply'}
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </section>
  );
}