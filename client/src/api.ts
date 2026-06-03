const API_BASE = "http://localhost:5167"; 

// ClientResponse
export interface Client {
  id: number;
  name: string;
  companyName: string | null;
  email: string | null;
  phone: string | null;
  address: string | null;
  notes: string | null;
  createdAt: string;
}

// CreateClientRequest
export interface NewClient {
  name: string;
  companyName?: string;
  email?: string;
  phone?: string;
  address?: string;
  notes?: string;
}

export async function getClients(): Promise<Client[]> {
  const res = await fetch(`${API_BASE}/api/clients`);
  if (!res.ok) throw new Error(`Failed to load clients (${res.status})`);
  return res.json();
}

export async function createClient(input: NewClient): Promise<Client> {
  const res = await fetch(`${API_BASE}/api/clients`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(input),
  });
  if (!res.ok) throw new Error(`Failed to create client (${res.status})`);
  return res.json();
}

export async function updateClient(id: number, input: NewClient): Promise<void> {
  const res = await fetch(`${API_BASE}/api/clients/${id}`, {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(input),
  });
  if (!res.ok) throw new Error(`Failed to update client (${res.status})`);
}

export async function deleteClient(id: number): Promise<void> {
  const res = await fetch(`${API_BASE}/api/clients/${id}`, {
    method: "DELETE",
  });
  if (!res.ok) throw new Error(`Failed to delete client (${res.status})`);
}