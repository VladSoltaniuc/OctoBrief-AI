import { Mail, Send, Loader2, CheckCircle } from 'lucide-react'

interface EmailSectionProps {
  email: string
  onEmailChange: (email: string) => void
  onSendEmail: () => void
  isSending: boolean
  isSent: boolean
  error: string | null
}

export function EmailSection({
  email,
  onEmailChange,
  onSendEmail,
  isSending,
  isSent,
  error,
}: EmailSectionProps) {
  return (
    <div className="bg-white rounded-lg shadow-sm border p-3 mt-4">
      <div className="flex items-center gap-3">
        <Mail className="w-4 h-4 text-gray-400 flex-shrink-0" />

        {!isSent ? (
          <>
            <input
              type="email"
              value={email}
              onChange={(e) => onEmailChange(e.target.value)}
              onKeyDown={(e) => {
                if (e.key === 'Enter') {
                  e.preventDefault()
                }
              }}
              placeholder="Send to email..."
              className="flex-1 px-3 py-1.5 border border-gray-200 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-transparent outline-none transition text-sm"
            />
            <button
              type="button"
              onClick={onSendEmail}
              disabled={isSending || !email.trim()}
              className="px-3 py-1.5 bg-primary-600 hover:bg-primary-700 text-white text-sm font-medium rounded-lg transition disabled:opacity-50 flex items-center gap-1.5"
            >
              {isSending ? (
                <Loader2 className="w-3.5 h-3.5 animate-spin" />
              ) : (
                <Send className="w-3.5 h-3.5" />
              )}
              Send
            </button>
          </>
        ) : (
          <div className="flex items-center gap-2 text-green-600">
            <CheckCircle className="w-4 h-4" />
            <span className="text-sm font-medium">Sent!</span>
          </div>
        )}
      </div>
      {error && <p className="mt-2 text-sm text-red-600 pl-7">{error}</p>}
    </div>
  )
}
