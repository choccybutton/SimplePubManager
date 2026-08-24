import React, { useEffect } from 'react';
import { useTasks } from '../../hooks/useTasks';
import { LoadingSpinner } from '../Common/LoadingSpinner';

export const TaskList: React.FC = () => {
  const { tasks, loading, error, fetchTasks } = useTasks();

  useEffect(() => {
    fetchTasks({ pageNumber: 1, pageSize: 10 });
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  if (loading) {
    return <LoadingSpinner message="Loading tasks..." />;
  }

  if (error) {
    return (
      <div style={{ padding: '2rem', color: 'red' }}>
        <h2>Error</h2>
        <p>{error}</p>
      </div>
    );
  }

  return (
    <div style={{ padding: '2rem' }}>
      <h2>Tasks</h2>
      {tasks.length === 0 ? (
        <p>No tasks found.</p>
      ) : (
        <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(300px, 1fr))', gap: '1rem' }}>
          {tasks.map((task) => (
            <div
              key={task.id}
              style={{
                padding: '1.5rem',
                backgroundColor: 'white',
                borderRadius: '0.5rem',
                boxShadow: '0 2px 4px rgba(0,0,0,0.1)',
                borderLeft: '4px solid #3498db',
              }}
            >
              <h4>{task.title}</h4>
              <p style={{ color: '#666', marginBottom: '0.5rem' }}>{task.description}</p>
              <div style={{ fontSize: '0.875rem', color: '#999' }}>
                <p>Due: {new Date(task.dueDate).toLocaleDateString()}</p>
                <p>Status: {task.status}</p>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
};
