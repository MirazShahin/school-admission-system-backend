import { useState } from 'react';
import CreateClassForm from '../components/CreateClassForm';
import ClassList from '../components/ClassList';

export default function AdminPage() {
  const [refresh, setRefresh] = useState(0);

  return (
    <main style={{ fontFamily: 'sans-serif' }}>
      <header style={{ padding: 16 }}>
        <h1>School Admin</h1>
      </header>
      <CreateClassForm onCreated={() => setRefresh((n) => n + 1)} />
      <ClassList key={refresh} studentId="" />
    </main>
  );
}