import { useEffect, useState } from 'react';
import { Plus, Check, Trash2, Circle, Loader2, CheckCircle2 } from 'lucide-react';
import service from './service';

function App() {
  const [newTodo, setNewTodo] = useState("");
  const [todos, setTodos] = useState([]);
  const [filter, setFilter] = useState('all');
  const [isLoading, setIsLoading] = useState(true);
  const [loadingTodos, setLoadingTodos] = useState(new Set());
  const [isAdding, setIsAdding] = useState(false);

  async function getTodos() {
    try {
      setIsLoading(true);
      const todos = await service.getTasks();
      setTodos(Array.isArray(todos) ? todos : []);
    } catch (error) {
      console.error('Failed to fetch todos:', error);
      setTodos([]);
    } finally {
      setIsLoading(false);
    }
  }

  async function createTodo(e) {
    e.preventDefault();
    if (!newTodo.trim()) return;

    setIsAdding(true);

    try {
      const added = await service.addTask(newTodo.trim());
      setTodos(prev => [...prev, added]);
      setNewTodo("");
    } catch (error) {
      console.error('Failed to create todo:', error);
    } finally {
      setIsAdding(false);
    }
  }

  async function updateCompleted(todo, isComplete) {
    setLoadingTodos(prev => new Set(prev).add(todo.id));
    try {
      await service.setCompleted(todo.id, isComplete);
      setTodos(prev =>
        prev.map(t => (t.id === todo.id ? { ...t, isComplete } : t))
      );
    } catch (error) {
      console.error('Failed to update todo:', error);
    } finally {
      setLoadingTodos(prev => {
        const newSet = new Set(prev);
        newSet.delete(todo.id);
        return newSet;
      });
    }
  }

  async function deleteTodo(id) {
    setLoadingTodos(prev => new Set(prev).add(id));
    try {
      await service.deleteTask(id);
      setTodos(prev => prev.filter(todo => todo.id !== id));
    } catch (error) {
      console.error('Failed to delete todo:', error);
    } finally {
      setLoadingTodos(prev => {
        const newSet = new Set(prev);
        newSet.delete(id);
        return newSet;
      });
    }
  }

  async function clearCompleted() {
    const completedTodos = todos.filter(todo => todo.isComplete);
    setLoadingTodos(prev => {
      const newSet = new Set(prev);
      completedTodos.forEach(todo => newSet.add(todo.id));
      return newSet;
    });
    try {
      await Promise.all(completedTodos.map(todo => service.deleteTask(todo.id)));
      setTodos(prevTodos => prevTodos.filter(todo => !todo.isComplete));
    } catch (error) {
      console.error('Failed to clear completed todos:', error);
    } finally {
      setLoadingTodos(prev => {
        const newSet = new Set(prev);
        completedTodos.forEach(todo => newSet.delete(todo.id));
        return newSet;
      });
    }
  }

  useEffect(() => {
    getTodos();
  }, []);

  const filteredTodos = todos.filter(todo => {
    if (filter === 'active') return !todo.isComplete;
    if (filter === 'completed') return todo.isComplete;
    return true;
  });

  const activeTodosCount = todos.filter(todo => !todo.isComplete).length;
  const completedTodosCount = todos.filter(todo => todo.isComplete).length;

  if (isLoading) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-violet-50 via-blue-50 to-indigo-100 flex items-center justify-center">
        <div className="text-center">
          <div className="w-16 h-16 border-4 border-violet-200 border-t-violet-600 rounded-full animate-spin mx-auto mb-4"></div>
          <p className="text-violet-600 font-medium">Loading your tasks...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-violet-50 via-blue-50 to-indigo-100 py-8 px-4">
      <div className="max-w-2xl mx-auto">
        <div className="text-center mb-12">
          <div className="inline-flex items-center justify-center w-20 h-20 bg-gradient-to-br from-violet-500 to-indigo-600 rounded-2xl mb-6 shadow-lg transform rotate-3">
            <CheckCircle2 size={36} className="text-white" />
          </div>
          <h1 className="text-6xl font-bold bg-gradient-to-r from-violet-600 via-blue-600 to-indigo-600 bg-clip-text text-transparent mb-4 tracking-tight">
            TodoFlow
          </h1>
          <p className="text-gray-600 text-lg font-medium">Turn tasks into achievements</p>
        </div>

        <div className="bg-white/80 backdrop-blur-lg rounded-3xl shadow-2xl border border-white/50 overflow-hidden">
          <div className="p-8 bg-gradient-to-r from-violet-500/5 to-indigo-500/5 border-b border-violet-100">
            <div className="relative">
              <div className="relative group">
                <input
                  className="w-full text-xl px-6 py-5 pr-16 rounded-2xl border-2 border-violet-200 focus:border-violet-400 focus:outline-none transition-all duration-300 placeholder-gray-400 bg-white/70 focus:bg-white shadow-lg focus:shadow-xl backdrop-blur-sm"
                  placeholder="What's on your mind today?"
                  value={newTodo}
                  onChange={(e) => setNewTodo(e.target.value)}
                  onKeyPress={(e) => e.key === 'Enter' && createTodo(e)}
                  autoFocus
                />
                <button
                  type="button"
                  onClick={createTodo}
                  disabled={!newTodo.trim() || isAdding}
                  className="absolute right-3 top-1/2 transform -translate-y-1/2 p-3 rounded-xl bg-gradient-to-r from-violet-500 to-indigo-600 text-white hover:from-violet-600 hover:to-indigo-700 disabled:from-gray-300 disabled:to-gray-400 disabled:cursor-not-allowed transition-all duration-300 shadow-lg hover:shadow-xl active:scale-95"
                >
                  {isAdding ? (
                    <Loader2 size={20} className="animate-spin" />
                  ) : (
                    <Plus size={20} />
                  )}
                </button>
              </div>
            </div>
          </div>

          {todos.length > 0 && (
            <div className="divide-y divide-violet-100/50">
              {filteredTodos.map(todo => {
                const isLoadingTodo = loadingTodos.has(todo.id);
                return (
                  <div
                    key={todo.id}
                    className={`group p-6 hover:bg-gradient-to-r hover:from-violet-50/50 hover:to-indigo-50/50 transition-all duration-300 ${
                      todo.isComplete ? 'bg-gradient-to-r from-green-50/50 to-emerald-50/50' : ''
                    } ${isLoadingTodo ? 'opacity-60' : ''}`}
                  >
                    <div className="flex items-center gap-4">
                      <button
                        onClick={() => updateCompleted(todo, !todo.isComplete)}
                        disabled={isLoadingTodo}
                        className={`flex-shrink-0 w-7 h-7 rounded-full border-2 transition-all duration-300 flex items-center justify-center disabled:cursor-not-allowed transform hover:scale-110 ${
                          todo.isComplete
                            ? 'border-emerald-400 bg-gradient-to-r from-emerald-400 to-green-500 shadow-lg'
                            : 'border-violet-300 hover:border-violet-400 hover:bg-violet-50'
                        }`}
                      >
                        {isLoadingTodo ? (
                          <Loader2 size={16} className="animate-spin text-violet-500" />
                        ) : todo.isComplete ? (
                          <Check size={14} className="text-white" />
                        ) : (
                          <div className="w-3 h-3 rounded-full bg-gradient-to-r from-violet-400 to-indigo-500 opacity-0 group-hover:opacity-100 transition-opacity duration-300" />
                        )}
                      </button>

                      <label
                        className={`flex-1 text-lg cursor-pointer transition-all duration-300 font-medium ${
                          todo.isComplete
                            ? 'text-gray-500 line-through decoration-2 decoration-emerald-400'
                            : 'text-gray-700 hover:text-gray-900'
                        } ${isLoadingTodo ? 'cursor-not-allowed' : ''}`}
                        onClick={() => !isLoadingTodo && updateCompleted(todo, !todo.isComplete)}
                      >
                        {todo.name}
                      </label>

                      <button
                        onClick={() => deleteTodo(todo.id)}
                        disabled={isLoadingTodo}
                        className="opacity-0 group-hover:opacity-100 p-2 text-red-400 hover:text-red-600 transition-all duration-300 hover:bg-red-50 rounded-xl disabled:cursor-not-allowed disabled:opacity-30 transform hover:scale-110"
                      >
                        {isLoadingTodo ? (
                          <Loader2 size={18} className="animate-spin" />
                        ) : (
                          <Trash2 size={18} />
                        )}
                      </button>
                    </div>
                  </div>
                );
              })}
            </div>
          )}

          {todos.length === 0 && (
            <div className="p-16 text-center">
              <div className="w-32 h-32 mx-auto mb-8 bg-gradient-to-br from-violet-100 to-indigo-100 rounded-full flex items-center justify-center shadow-lg">
                <CheckCircle2 size={48} className="text-violet-400" />
              </div>
              <h3 className="text-2xl font-bold text-gray-600 mb-3">Ready to get things done?</h3>
              <p className="text-gray-500 text-lg">Add your first task above and start building momentum!</p>
            </div>
          )}

          {/* Footer */}
          {todos.length > 0 && (
            <div className="p-6 bg-gradient-to-r from-violet-50/50 to-indigo-50/50 border-t border-violet-100">
              <div className="flex flex-col sm:flex-row items-center justify-between gap-4">
                <div className="text-sm font-medium text-gray-600 bg-white/60 px-4 py-2 rounded-full backdrop-blur-sm">
                  <span className="text-violet-600 font-bold">{activeTodosCount}</span>
                  {activeTodosCount === 1 ? ' task' : ' tasks'} remaining
                </div>

                <div className="flex gap-1 bg-white/60 p-1 rounded-xl backdrop-blur-sm shadow-sm">
                  {[
                    { key: 'all', label: 'All', count: todos.length },
                    { key: 'active', label: 'Active', count: activeTodosCount },
                    { key: 'completed', label: 'Done', count: completedTodosCount }
                  ].map(({ key, label, count }) => (
                    <button
                      key={key}
                      onClick={() => setFilter(key)}
                      className={`px-4 py-2 rounded-lg text-sm font-medium transition-all duration-300 ${
                        filter === key
                          ? 'bg-gradient-to-r from-violet-500 to-indigo-600 text-white shadow-lg transform scale-105'
                          : 'text-gray-600 hover:text-violet-600 hover:bg-violet-50'
                      }`}
                    >
                      {label} {count > 0 && (
                        <span className={`ml-1 text-xs px-1.5 py-0.5 rounded-full ${
                          filter === key ? 'bg-white/20' : 'bg-violet-100 text-violet-600'
                        }`}>
                          {count}
                        </span>
                      )}
                    </button>
                  ))}
                </div>

                {completedTodosCount > 0 && (
                  <button
                    onClick={clearCompleted}
                    className="text-sm font-medium text-red-500 hover:text-red-700 bg-red-50 hover:bg-red-100 px-4 py-2 rounded-full transition-all duration-300 transform hover:scale-105"
                  >
                    Clear completed ({completedTodosCount})
                  </button>
                )}
              </div>
            </div>
          )}
        </div>

        {todos.length > 0 && (
          <div className="mt-8 bg-white/60 backdrop-blur-sm rounded-2xl p-6 shadow-lg">
            <div className="flex items-center justify-between mb-3">
              <span className="text-sm font-medium text-gray-600">Progress</span>
              <span className="text-sm font-bold text-violet-600">
                {Math.round((completedTodosCount / todos.length) * 100)}%
              </span>
            </div>
            <div className="w-full bg-gray-200 rounded-full h-3 overflow-hidden">
              <div
                className="h-full bg-gradient-to-r from-violet-500 to-indigo-600 rounded-full transition-all duration-700 ease-out shadow-sm"
                style={{ width: `${(completedTodosCount / todos.length) * 100}%` }}
              />
            </div>
          </div>
        )}
      </div>
    </div>
  );
}

export default App;