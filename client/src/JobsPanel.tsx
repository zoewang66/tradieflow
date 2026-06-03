import { useEffect, useState, type FormEvent } from "react";
import {
  getJobs, createJob, updateJob, deleteJob,
  type Client, type Job, type JobStatus,
} from "./api";

const STATUSES: JobStatus[] = ["Quoted", "Scheduled", "InProgress", "Completed", "Cancelled"];

export function JobsPanel({ client }: { client: Client }) {
  const [jobs, setJobs] = useState<Job[]>([]);
  const [title, setTitle] = useState("");
  const [status, setStatus] = useState<JobStatus>("Quoted");
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  // reload jobs whenever the selected client changes
  useEffect(() => { refresh(); }, [client.id]);

  async function refresh() {
    try { setJobs(await getJobs(client.id)); }
    catch (e) { setError((e as Error).message); }
  }

  async function handleAdd(e: FormEvent) {
    e.preventDefault(); setError(null); setLoading(true);
    try {
      await createJob({ clientId: client.id, title, status });
      setTitle(""); setStatus("Quoted");
      await refresh();
    } catch (err) { setError((err as Error).message); }
    finally { setLoading(false); }
  }

  async function handleStatusChange(job: Job, newStatus: JobStatus) {
    setError(null);
    try {
      // PUT replaces the whole job, so resend its other fields
      await updateJob(job.id, {
        title: job.title,
        description: job.description ?? undefined,
        status: newStatus,
        scheduledAt: job.scheduledAt ?? undefined,
      });
      await refresh();
    } catch (err) { setError((err as Error).message); }
  }

  async function handleDelete(id: number) {
    setError(null);
    try { await deleteJob(id); await refresh(); }
    catch (err) { setError((err as Error).message); }
  }

  return (
    <section className="card jobs-panel">
      <h2>Jobs for {client.name}</h2>

      <form onSubmit={handleAdd} className="field-row">
        <input className="input" placeholder="Job title (required)"
               value={title} onChange={(e) => setTitle(e.target.value)} />
        <select className="input" value={status}
                onChange={(e) => setStatus(e.target.value as JobStatus)}>
          {STATUSES.map((s) => <option key={s} value={s}>{s}</option>)}
        </select>
        <button type="submit" className="btn btn-primary" disabled={loading}>
          {loading ? "Saving…" : "Add job"}
        </button>
      </form>

      {error && <div className="error">{error}</div>}

      {jobs.length === 0 ? (
        <div className="empty">No jobs yet for this client.</div>
      ) : (
        <ul className="job-list">
          {jobs.map((j) => (
            <li key={j.id} className="job">
              <div className="job-info">
                <div className="job-title">{j.title}</div>
                {j.description && <div className="job-desc">{j.description}</div>}
              </div>
              <select
                className={`status-select status-${j.status.toLowerCase()}`}
                value={j.status}
                onChange={(e) => handleStatusChange(j, e.target.value as JobStatus)}
              >
                {STATUSES.map((s) => <option key={s} value={s}>{s}</option>)}
              </select>
              <button className="btn btn-sm btn-delete" onClick={() => handleDelete(j.id)}>Delete</button>
            </li>
          ))}
        </ul>
      )}
    </section>
  );
}