import { AlertCircle } from 'lucide-react'

interface ErrorStateProps {
  message: string
  onRetry: () => void
}

export function ErrorState({ message, onRetry }: ErrorStateProps) {
  return (
    <div className="max-w-xl mx-auto">
      <div className="bg-white rounded-2xl shadow-lg p-8 text-center">
        <AlertCircle className="w-12 h-12 text-red-500 mx-auto mb-4" />
        <h3 className="text-xl font-bold text-gray-900 mb-2">Something went wrong</h3>
        <p className="text-gray-600 mb-6">{message}</p>
        <button
          onClick={onRetry}
          className="px-6 py-3 bg-primary-600 hover:bg-primary-700 text-white font-medium rounded-xl transition"
        >
          Try Again
        </button>
      </div>
    </div>
  )
}
