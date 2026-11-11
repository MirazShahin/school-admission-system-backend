import { useState } from 'react';
import StudentIdBox from '../components/StudentIdBox';
import ClassChooser from '../components/ClassChooser';
import MyHolds from '../components/MyHolds';

export default function GuardianPage() {
  const [studentId, setStudentId] = useState(
    () => localStorage.getItem('studentId') ?? '',
  );

  return (
    <main style={{ fontFamily: 'sans-serif' }}>
      <header style={{ padding: 16 }}>
        <h1>School Admission</h1>
      </header>
      <StudentIdBox onChange={setStudentId} />
      <ClassChooser studentId={studentId} />
      <MyHolds studentId={studentId} />
    </main>
  );
}