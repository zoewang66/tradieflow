import { useEffect, useState, type FormEvent } from "react";
import {
  getInvoices, createInvoice, updateInvoiceStatus, deleteInvoice,
  type Job, type Invoice, type InvoiceStatus, type NewLineItem,
} from "./api";

const STATUSES: InvoiceStatus[] = ["Draft", "Sent", "Paid", "Cancelled"];

type Row = { description: string; quantity: string; unitPrice: string };
const emptyRow = (): Row => ({ description: "", quantity: "1", unitPrice: "0" });
const money = (n: number) => `$${n.toFixed(2)}`;

// client-side preview that mirrors the server's GST math (server is still the source of truth)
function calcTotals(rows: Row[]) {
  const subtotal = Math.round(
    rows.reduce((s, r) => s + (Number(r.quantity) || 0) * (Number(r.unitPrice) || 0), 0) * 100) / 100;
  const gst = Math.round(subtotal * 0.1 * 100) / 100;
  return { subtotal, gst, total: subtotal + gst };
}

export function InvoicesPanel({ job }: { job: Job }) {
  const [invoices, setInvoices] = useState<Invoice[]>([]);
  const [rows, setRows] = useState<Row[]>([emptyRow()]);
  const [notes, setNotes] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  useEffect(() => { refresh(); }, [job.id]);

  async function refresh() {
    try { setInvoices(await getInvoices(job.id)); }
    catch (e) { setError((e as Error).message); }
  }

  const updateRow = (i: number, field: keyof Row, value: string) =>
    setRows(rows.map((r, idx) => (idx === i ? { ...r, [field]: value } : r)));
  const addRow = () => setRows([...rows, emptyRow()]);
  const removeRow = (i: number) => setRows(rows.length === 1 ? rows : rows.filter((_, idx) => idx !== i));

  async function handleCreate(e: FormEvent) {
    e.preventDefault(); setError(null); setLoading(true);
    try {
      const lineItems: NewLineItem[] = rows.map((r) => ({
        description: r.description,
        quantity: Number(r.quantity) || 0,
        unitPrice: Number(r.unitPrice) || 0,
      }));
      await createInvoice({ jobId: job.id, notes: notes || undefined, lineItems });
      setRows([emptyRow()]); setNotes("");
      await refresh();
    } catch (err) { setError((err as Error).message); }
    finally { setLoading(false); }
  }

  async function handleStatusChange(inv: Invoice, status: InvoiceStatus) {
    setError(null);
    try { await updateInvoiceStatus(inv.id, status); await refresh(); }
    catch (err) { setError((err as Error).message); }
  }
  async function handleDelete(id: number) {
    setError(null);
    try { await deleteInvoice(id); await refresh(); }
    catch (err) { setError((err as Error).message); }
  }

  const preview = calcTotals(rows);

  return (
    <section className="card invoices-panel">
      <h3>Invoices for “{job.title}”</h3>

      <form onSubmit={handleCreate} className="invoice-form">
        {rows.map((r, i) => (
          <div className="li-row" key={i}>
            <input className="input li-desc" placeholder="Description"
                   value={r.description} onChange={(e) => updateRow(i, "description", e.target.value)} />
            <input className="input li-qty" type="number" min="0" step="0.01" placeholder="Qty"
                   value={r.quantity} onChange={(e) => updateRow(i, "quantity", e.target.value)} />
            <input className="input li-price" type="number" min="0" step="0.01" placeholder="Unit $"
                   value={r.unitPrice} onChange={(e) => updateRow(i, "unitPrice", e.target.value)} />
            <span className="li-total">{money((Number(r.quantity) || 0) * (Number(r.unitPrice) || 0))}</span>
            <button type="button" className="btn btn-sm btn-ghost" onClick={() => removeRow(i)}>✕</button>
          </div>
        ))}
        <button type="button" className="btn btn-sm btn-ghost add-row" onClick={addRow}>+ Add line</button>

        <div className="totals-preview">
          <span>Subtotal {money(preview.subtotal)}</span>
          <span>GST {money(preview.gst)}</span>
          <strong>Total {money(preview.total)}</strong>
        </div>

        <button type="submit" className="btn btn-primary" disabled={loading}>
          {loading ? "Creating…" : "Create invoice"}
        </button>
      </form>

      {error && <div className="error">{error}</div>}

      {invoices.length === 0 ? (
        <div className="empty">No invoices yet for this job.</div>
      ) : (
        <ul className="invoice-list">
          {invoices.map((inv) => (
            <li key={inv.id} className="invoice">
              <div className="invoice-head">
                <strong>{inv.invoiceNumber}</strong>
                <select className={`status-select inv-${inv.status.toLowerCase()}`} value={inv.status}
                        onChange={(e) => handleStatusChange(inv, e.target.value as InvoiceStatus)}>
                  {STATUSES.map((s) => <option key={s} value={s}>{s}</option>)}
                </select>
                <button className="btn btn-sm btn-delete" onClick={() => handleDelete(inv.id)}>Delete</button>
              </div>
              <ul className="invoice-lines">
                {inv.lineItems.map((li) => (
                  <li key={li.id}>
                    <span>{li.description}</span>
                    <span>{li.quantity} × {money(li.unitPrice)}</span>
                    <span>{money(li.lineTotal)}</span>
                  </li>
                ))}
              </ul>
              <div className="invoice-totals">
                <span>Subtotal {money(inv.subtotal)}</span>
                <span>GST {money(inv.gst)}</span>
                <strong>Total {money(inv.total)}</strong>
              </div>
            </li>
          ))}
        </ul>
      )}
    </section>
  );
}