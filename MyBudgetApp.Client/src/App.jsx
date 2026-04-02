import { useState } from 'react';
import Dashboard from './components/Dashboard';
import Transactions from './components/Transactions';
import Categories from './components/Categories';
import './App.css';

const TABS = ['Dashboard', 'Transactions', 'Categories'];

function App() {
  const [activeTab, setActiveTab] = useState('Dashboard');

  return (
    <div className="app">
      <header className="app-header">
        <h1>💰 MyBudgetApp</h1>
        <nav>
          {TABS.map((tab) => (
            <button
              key={tab}
              className={activeTab === tab ? 'active' : ''}
              onClick={() => setActiveTab(tab)}
            >
              {tab}
            </button>
          ))}
        </nav>
      </header>

      <main className="app-main">
        {activeTab === 'Dashboard' && <Dashboard />}
        {activeTab === 'Transactions' && <Transactions />}
        {activeTab === 'Categories' && <Categories />}
      </main>
    </div>
  );
}

export default App;

