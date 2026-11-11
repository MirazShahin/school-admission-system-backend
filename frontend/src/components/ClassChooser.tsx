import { useEffect, useState } from 'react';
import { api, ApiError } from '../api';
import type { ClassItem } from '../types';

export default function ClassChooser({
  studentId,
  onApplied,
}: {
  studentId: string;
  onApplied?: () => void;
}) {
  const [classes, setClasses] = useState<ClassItem[]>([]);
  const [selected, setSelected] = useState('');
  const [message, setMessage] = useState('');
  const [error, setError] = useState('');
  const [busy, setBusy] = useState(false);

  // Refresh seat counts every 3 seconds.
  useEffect(() => {
    let alive = true;
    const load = () =>
      api<ClassItem[]>('/classes')
        .then((rows) => alive && setClasses(rows))
        .catch((e) => alive && setError(e instanceof Error ? e.message : 'Could not load classes.'));
    load();
    const timer = setInterval(load, 3000);
    return () => {
      alive = false;
      clearInterval(timer);
    };
  }, []);

  async function apply() {
    setBusy(true);
    setError('');
    setMessage('');
    try {
      await api(`/classes/${selected}/holds`, { method: 'POST', studentId });
      setMessage('Seat hold hoyeche. Niche "My applications" theke bhorti fee din.');
      setSelected('');
      onApplied?.(); // refresh "My applications" immediately
    } catch (e) {
      setError(e instanceof ApiError ? e.message : 'Could not apply.');
    } finally {
      setBusy(false);
    }
  }

  if (!studentId) return <p style={{ padding: 16 }}>Upore Student ID din.</p>;

  return (
    <section style={{ padding: 16 }}>
      <h2>Class choose korun</h2>
      <ul style={{ listStyle: 'none', padding: 0 }}>
        {classes.map((c) => {
          const full = c.availableSeats <= 0;
          return (
            <li
              key={c.id}
              style={{ padding: 8, border: '1px solid #ddd', marginBottom: 6, opacity: full ? 0.5 : 1 }}
            >
              <label>
                <input
                  type="radio"
                  name="class"
                  value={c.id}
                  disabled={full}
                  checked={selected === c.id}
                  onChange={() => setSelected(c.id)}
                />{' '}
                <strong>{c.name}</strong> — available: {c.availableSeats} / {c.totalSeats}
                {full && ' (Full)'}
              </label>
            </li>
          );
        })}
      </ul>
      <button disabled={!selected || busy} onClick={apply}>
        Apply
      </button>
      {message && <p style={{ color: 'green' }}>{message}</p>}
      {error && <p style={{ color: 'crimson' }}>{error}</p>}
    </section>
  );
}