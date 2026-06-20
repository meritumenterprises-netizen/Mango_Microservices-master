export function Loader({ message = 'Loading...' }: { message?: string }) {
  return (
    <div className="loader-wrap" role="status" aria-live="polite">
      <div className="spinner-border text-primary" />
      <span>{message}</span>
    </div>
  );
}
