import { useState } from 'react';
import { api, ApiError } from '../api';

export default function CreateClassForm({ onCreated }: { onCreated: () => void }) {
  const [name, setName] = useState('');
  const [totalSeats, setTotalSeats] = useState(10);
  const [error, setError] = useState('');
  const [saved, setSaved] = useState(false);

  async function submit(e: React.FormEvent) {
    e.preventDefault();
    setError('');
    setSaved(false);
    try {
      await api('/classes', { method: 'POST', body: { name, totalSeats } });
      setSaved(true);
      setName('');
      onCreated();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : 'Could not create class.');
    }
  }

  return (
    <section style={{ padding: 16 }}>
      <h2>Create class (admin)</h2>
      <form onSubmit={submit}>
        <input
          placeholder="Class name, e.g. Class 6 - Morning Shift"
          value={name}
          onChange={(e) => setName(e.target.value)}
          style={{ width: 320 }}
          required
        />{' '}
        <input
          type="number"
          min={1}
          value={totalSeats}
          onChange={(e) => setTotalSeats(Number(e.target.value))}
          style={{ width: 100 }}
          required
        />{' '}
        <button type="submit">Create</button>
      </form>
      {error && <p style={{ color: 'crimson' }}>{error}</p>}
      {saved && <p style={{ color: 'green' }}>Class created.</p>}
    </section>
  );
}