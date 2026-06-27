import { useEffect, useState } from 'react';
import apiClient from '../api/apiClient';
import type { ApiResponse, TaskSummary } from '../types';
import { TaskStatusLabel, PriorityLabel } from '../types';

export default function SummaryPanel() {
  const [rows, setRows] = useState<TaskSummary[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    apiClient
      .get<ApiResponse<TaskSummary[]>>('/api/tasks/summary')
      .then((res) => setRows(res.data.data ?? []))
      .catch(() => setError('Failed to load summary.'))
      .finally(() => setLoading(false));
  }, []);

  if (loading) return <div className="spinner">Loading summary…</div>;
  if (error) return <p className="error-text">{error}</p>;

  return (
    <div className="card summary-panel">
      <h2>Summary</h2>
      {rows.length === 0 ? (
        <p>No data.</p>
      ) : (
        <table className="summary-table">
          <thead>
            <tr>
              <th>Status</th>
              <th>Priority</th>
              <th>Count</th>
            </tr>
          </thead>
          <tbody>
            {rows.map((r, i) => (
              <tr key={i}>
                <td>{TaskStatusLabel[r.status] ?? r.status}</td>
                <td>{PriorityLabel[r.priority] ?? r.priority}</td>
                <td>{r.count}</td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}
