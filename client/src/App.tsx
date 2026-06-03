import { useEffect, useState, type FormEvent } from "react";
import { getClients, createClient, updateClient, deleteClient, type NewClient, type Client } from "./api";

function App() {
  const [clients, setClients] = useState<Client[]>([]);
  const [name, setName] = useState("");
  const [email, setEmail] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);
  const [editing, setEditing] = useState<Client | null>(null);


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
        // PUT 会整体替换这条记录,所以把表单没显示的字段也带上,避免被清空
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
      if (editing?.id === id) cancelEdit(); // 如果删的正是在编辑的那条,清空表单
      await refresh();
    } catch (err) {
      setError((err as Error).message);
    }
  }

  return (
    <div style={{ maxWidth: 600, margin: "2rem auto", fontFamily: "sans-serif" }}>
      <h1>Clients</h1>

      <form onSubmit={handleSubmit} style={{ marginBottom: "1.5rem" }}>
        <input placeholder="Name (required)" value={name}
               onChange={(e) => setName(e.target.value)} />
        <input placeholder="Email" value={email}
               onChange={(e) => setEmail(e.target.value)} />
        <button type="submit" disabled={loading}>
          {loading ? "Saving..." : editing ? "Update" : "Add client"}
        </button>
        {editing && (
          <button type="button" onClick={cancelEdit} disabled={loading}>
            Cancel
          </button>
        )}
      </form>

      {error && <p style={{ color: "red" }}>{error}</p>}

      <ul>
        {clients.map((c) => (
          <li key={c.id} style={{ marginBottom: "0.5rem" }}>
            <strong>{c.name}</strong>{c.email ? ` — ${c.email}` : ""}{" "}
            <button onClick={() => startEdit(c)}>Edit</button>{" "}
            <button onClick={() => handleDelete(c.id)}>Delete</button>
          </li>
        ))}
      </ul>
    </div>
  );
}

export default App;