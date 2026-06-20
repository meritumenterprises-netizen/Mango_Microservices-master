import type { ReactNode } from 'react';

export function PageTitle({ title, action }: { title: string; action?: ReactNode }) {
  return (
    <div className="page-title">
      <h1>{title}</h1>
      {action}
    </div>
  );
}
