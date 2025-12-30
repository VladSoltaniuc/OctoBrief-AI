import { COUNTRIES } from '../constants'
import { PreviewBriefResponse } from '../api'
import { EmailSection } from './EmailSection'
import parse from 'html-react-parser'
import DOMPurify from 'dompurify'

interface ResultsViewProps {
  result: PreviewBriefResponse
  topic: string
  country: string
  email: string
  onEmailChange: (email: string) => void
  onSendEmail: () => void
  isSendingEmail: boolean
  emailSent: boolean
  emailError: string | null
  onReset: () => void
}

export function ResultsView({
  result,
  topic,
  country,
  email,
  onEmailChange,
  onSendEmail,
  isSendingEmail,
  emailSent,
  emailError,
  onReset,
}: ResultsViewProps) {
  const countryLabel = COUNTRIES.find((c) => c.value === country)?.label || country
  const successfulSources = result.sources

  // sanitize the HTML from the server and parse into React nodes
  const sanitized = DOMPurify.sanitize(result.htmlContent || '')
  const parsed = parse(sanitized)

  return (
    <div className="space-y-4">
      {/* Header with New Search */}
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-3">
          <button
            onClick={onReset}
            className="text-gray-400 hover:text-gray-600 transition"
            title="New Search"
          >
            ←
          </button>
          <div>
            <h1 className="text-xl font-bold text-gray-900">{topic} News</h1>
            <p className="text-gray-500 text-xs">
              {successfulSources} sources • {countryLabel}
            </p>
          </div>
        </div>
      </div>

      {/* News Cards */}
      <div className="news-grid">{parsed}</div>

      {/* Email Section */}
      <EmailSection
        email={email}
        onEmailChange={onEmailChange}
        onSendEmail={onSendEmail}
        isSending={isSendingEmail}
        isSent={emailSent}
        error={emailError}
      />
    </div>
  )
}
