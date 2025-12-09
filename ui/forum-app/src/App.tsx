import {BrowserRouter, Routes, Route, Navigate} from 'react-router-dom'
import {AuthProvider} from '@/contexts/AuthContext'
import {ProtectedRoute} from '@/components/ProtectedRoute'
import {QuestionsPage} from '@/pages/QuestionsPage'
import {CreateQuestionPage} from '@/pages/CreateQuestionPage'
import {AuthPage} from "@/pages/AuthPage";

function App() {
    return (
        <BrowserRouter>
            <AuthProvider>
                <Routes>
                    <Route path="/login" element={<AuthPage/>}/>
                    <Route
                        path="/questions"
                        element={
                            <ProtectedRoute>
                                <QuestionsPage/>
                            </ProtectedRoute>
                        }
                    />
                    <Route
                        path="/questions/create"
                        element={
                            <ProtectedRoute>
                                <CreateQuestionPage/>
                            </ProtectedRoute>
                        }
                    />
                    <Route path="/" element={<Navigate to="/questions" replace/>}/>
                    <Route path="*" element={<Navigate to="/questions" replace/>}/>
                </Routes>
            </AuthProvider>
        </BrowserRouter>
    )
}

export default App
