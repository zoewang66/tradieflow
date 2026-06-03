import { useEffect, useState, type FormEvent } from "react";
import { getClients, createClient, type Client } from "./api";

function App() {
  const [clients, setClients] = useState<Client[]>([]);
  const [name, setName] = useState("");
  const [email, setEmail] = useState("");
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

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setError(null);
    setLoading(true);
    try {
      await createClient({ name, email: email || undefined });
      setName("");
      setEmail("");
      await refresh();
    } catch (err) {
      setError((err as Error).message);
    } finally {
      setLoading(false);
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
          {loading ? "Saving..." : "Add client"}
        </button>
      </form>

      {error && <p style={{ color: "red" }}>{error}</p>}

      <ul>
        {clients.map((c) => (
          <li key={c.id}>
            <strong>{c.name}</strong>{c.email ? ` — ${c.email}` : ""}
          </li>
        ))}
      </ul>
    </div>
  );
}

export default App;