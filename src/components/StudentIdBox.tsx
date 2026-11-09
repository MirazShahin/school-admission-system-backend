import { useState } from 'react';

const KEY = 'studentId';

export function getStudentId(): string {
  return localStorage.getItem(KEY) ?? '';
}

export default function StudentIdBox({ onChange }: { onChange: (id: string) => void }) {
  const [value, setValue] = useState(getStudentId());
  const valid = /^\d{17}$/.test(value);

  return (
    <div style={{ padding: 12, borderBottom: '1px solid #ddd' }}>
      <label>
        Student ID (17 digits):{' '}
        <input
          value={value}
          maxLength={17}
          onChange={(e) => {
            const v = e.target.value;
            setValue(v);
            localStorage.setItem(KEY, v);
            onChange(v);
          }}
          style={{ width: 220 }}
        />
      </label>
      {value && !valid && <span style={{ color: 'crimson', marginLeft: 8 }}>Must be exactly 17 digits</span>}
    </div>
  );
}