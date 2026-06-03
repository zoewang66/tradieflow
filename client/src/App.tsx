import { useEffect, useState, type FormEvent } from "react";
import {
  getClients,
  createClient,
  updateClient,
  deleteClient,
  type Client,
  type NewClient,
} from "./api";
import { JobsPanel } from "./JobsPanel";
import "./App.css";

function App() {
  const [clients, setClients] = useState<Client[]>([]);
  const [name, setName] = useState("");
  const [email, setEmail] = useState("");
  const [editing, setEditing] = useState<Client | null>(null);
  const [selectedClient, setSelectedClient] = useState<Client | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    refresh();
  }, []);

  async function refresh() {
    try {
      setClients(await getClients());
    } catch (e) {
      setError((e as Error).message);
    }
  }

  function startEdit(client: Client) {
    setEditing(client);
    setName(client.name);
    setEmail(client.email ?? "");
    setError(null);
  }
  function cancelEdit() {
    setEditing(null);
    setName("");
    setEmail("");
  }

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setError(null);
    setLoading(true);
    try {
      if (editing) {
        const payload: NewClient = {
          name,
          email: email || undefined,
          companyName: editing.companyName ?? undefined,
          phone: editing.phone ?? undefined,
          address: editing.address ?? undefined,
          notes: editing.notes ?? undefined,
        };
        await updateClient(editing.id, payload);
      } else {
        await createClient({ name, email: email || undefined });
      }
      cancelEdit();
      await refresh();
    } catch (err) {
      setError((err as Error).message);
    } finally {
      setLoading(false);
    }
  }

  async function handleDelete(id: number) {
    setError(null);
    try {
      await deleteClient(id);
      if (editing?.id === id) cancelEdit();
      if (selectedClient?.id === id) setSelectedClient(null);
      await refresh();
    } catch (err) {
      setError((err as Error).message);
    }
  }

  return (
    <div className="app">
      <header className="app-header">
        <div className="app-logo">T</div>
        <div>
          <div className="app-title">TradieFlow</div>
          <div className="app-subtitle">Client &amp; job manager</div>
        </div>
      </header>

      <section className="card form-card">
        <h2>{editing ? "Edit client" : "Add a client"}</h2>
        <form onSubmit={handleSubmit}>
          <div className="field-row">
            <input
              className="input"
              placeholder="Name (required)"
              value={name}
              onChange={(e) => setName(e.target.value)}
            />
            <input
              className="input"
              placeholder="Email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
            />
            <button
              type="submit"
              className="btn btn-primary"
              disabled={loading}
            >
              {loading ? "Saving…" : editing ? "Update" : "Add client"}
            </button>
            {editing && (
              <button
                type="button"
                className="btn btn-ghost"
                onClick={cancelEdit}
                disabled={loading}
              >
                Cancel
              </button>
            )}
          </div>
        </form>
      </section>

      {error && <div className="error">{error}</div>}

      <div className="list-head">
        <h2>Clients</h2>
        <span className="count-badge">{clients.length}</span>
      </div>

      {clients.length === 0 ? (
        <div className="empty">No clients yet — add your first one above.</div>
      ) : (
        <ul className="client-list">
          {clients.map((c) => (
            <li
              key={c.id}
              className={
                "card client" +
                (selectedClient?.id === c.id ? " client-selected" : "")
              }
            >
              <div className="client-info">
                <div className="client-name">{c.name}</div>
                {c.email && <div className="client-email">{c.email}</div>}
              </div>
              <div className="client-actions">
                <button
                  className="btn btn-sm btn-jobs"
                  onClick={() => setSelectedClient(c)}
                >
                  Jobs
                </button>
                <button
                  className="btn btn-sm btn-edit"
                  onClick={() => startEdit(c)}
                >
                  Edit
                </button>
                <button
                  className="btn btn-sm btn-delete"
                  onClick={() => handleDelete(c.id)}
                >
                  Delete
                </button>
              </div>
            </li>
          ))}
        </ul>
      )}

      {selectedClient && <JobsPanel client={selectedClient} />}
    </div>
  );
}

export default App;
