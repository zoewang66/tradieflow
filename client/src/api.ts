const API_BASE = import.meta.env.VITE_API_URL ?? "http://localhost:5167";

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

export async function updateClient(
  id: number,
  input: NewClient,
): Promise<void> {
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

export type JobStatus =
  | "Quoted"
  | "Scheduled"
  | "InProgress"
  | "Completed"
  | "Cancelled";

export interface Job {
  id: number;
  clientId: number;
  clientName: string;
  title: string;
  description: string | null;
  status: JobStatus;
  scheduledAt: string | null;
  createdAt: string;
}
export interface NewJob {
  clientId: number;
  title: string;
  description?: string;
  status?: JobStatus;
  scheduledAt?: string;
}
export interface UpdateJob {
  title: string;
  description?: string;
  status: JobStatus;
  scheduledAt?: string;
}

export async function getJobs(clientId: number): Promise<Job[]> {
  const res = await fetch(`${API_BASE}/api/jobs?clientId=${clientId}`);
  if (!res.ok) throw new Error(`Failed to load jobs (${res.status})`);
  return res.json();
}
export async function createJob(input: NewJob): Promise<Job> {
  const res = await fetch(`${API_BASE}/api/jobs`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(input),
  });
  if (!res.ok) throw new Error(`Failed to create job (${res.status})`);
  return res.json();
}
export async function updateJob(id: number, input: UpdateJob): Promise<void> {
  const res = await fetch(`${API_BASE}/api/jobs/${id}`, {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(input),
  });
  if (!res.ok) throw new Error(`Failed to update job (${res.status})`);
}
export async function deleteJob(id: number): Promise<void> {
  const res = await fetch(`${API_BASE}/api/jobs/${id}`, { method: "DELETE" });
  if (!res.ok) throw new Error(`Failed to delete job (${res.status})`);
}

export type InvoiceStatus = "Draft" | "Sent" | "Paid" | "Cancelled";

export interface LineItem {
  id: number;
  description: string;
  quantity: number;
  unitPrice: number;
  lineTotal: number;
}
export interface Invoice {
  id: number;
  jobId: number;
  invoiceNumber: string;
  status: InvoiceStatus;
  issuedAt: string;
  dueAt: string | null;
  notes: string | null;
  createdAt: string;
  lineItems: LineItem[];
  subtotal: number;
  gst: number;
  total: number;
}
export interface NewLineItem {
  description: string;
  quantity: number;
  unitPrice: number;
}
export interface NewInvoice {
  jobId: number;
  notes?: string;
  dueAt?: string;
  lineItems: NewLineItem[];
}

export async function getInvoices(jobId: number): Promise<Invoice[]> {
  const res = await fetch(`${API_BASE}/api/invoices?jobId=${jobId}`);
  if (!res.ok) throw new Error(`Failed to load invoices (${res.status})`);
  return res.json();
}
export async function createInvoice(input: NewInvoice): Promise<Invoice> {
  const res = await fetch(`${API_BASE}/api/invoices`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(input),
  });
  if (!res.ok) throw new Error(`Failed to create invoice (${res.status})`);
  return res.json();
}
export async function updateInvoiceStatus(
  id: number,
  status: InvoiceStatus,
): Promise<void> {
  const res = await fetch(`${API_BASE}/api/invoices/${id}/status`, {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ status }),
  });
  if (!res.ok) throw new Error(`Failed to update invoice (${res.status})`);
}
export async function deleteInvoice(id: number): Promise<void> {
  const res = await fetch(`${API_BASE}/api/invoices/${id}`, {
    method: "DELETE",
  });
  if (!res.ok) throw new Error(`Failed to delete invoice (${res.status})`);
}
