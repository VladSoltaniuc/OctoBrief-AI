import { AlertCircle } from 'lucide-react'

interface ErrorBannerProps {
  message: string
}

export function ErrorBanner({ message }: ErrorBannerProps) {
  return (
    <div className="mb-6 p-3 rounded-lg bg-red-50 border border-red-200 flex items-center gap-2">
      <AlertCircle className="w-5 h-5 text-red-500 flex-shrink-0" />
      <p className="text-red-700 text-sm">{message}</p>
    </div>
  )
}
