class TaskManager {
    constructor() {
        this.tasks = [];
        this.filteredTasks = [];
        this.currentFilter = 'all';
        this.currentSort = 'date-newest';
        this.selectedTasks = new Set();
        this.init();
    }

    init() {
        this.loadTasks();
        this.attachEventListeners();
        this.restoreSettings();
        this.render();
        this.registerServiceWorker();
    }

    // Local Storage
    loadTasks() {
        try {
            const saved = localStorage.getItem('tasks');
            this.tasks = saved ? JSON.parse(saved) : [];
        } catch (e) {
            console.error('Error loading tasks:', e);
            this.tasks = [];
        }
    }

    saveTasks() {
        try {
            localStorage.setItem('tasks', JSON.stringify(this.tasks));
            this.updateStats();
        } catch (e) {
            console.error('Error saving tasks:', e);
            this.showToast('Error saving tasks', 'error');
        }
    }

    saveSetting(key, value) {
        try {
            localStorage.setItem(`setting_${key}`, JSON.stringify(value));
        } catch (e) {
            console.error('Error saving setting:', e);
        }
    }

    getSetting(key, defaultValue) {
        try {
            const saved = localStorage.getItem(`setting_${key}`);
            return saved ? JSON.parse(saved) : defaultValue;
        } catch (e) {
            return defaultValue;
        }
    }

    restoreSettings() {
        const darkMode = this.getSetting('darkMode', false);
        const notifications = this.getSetting('notifications', true);
        const sound = this.getSetting('sound', true);
        const autoArchive = this.getSetting('autoArchive', false);
        const sortBy = this.getSetting('sortBy', 'date-newest');

        if (darkMode) {
            document.body.classList.add('dark-mode');
            document.getElementById('darkModeBtn').textContent = '☀️';
        }

        document.getElementById('notificationsToggle').checked = notifications;
        document.getElementById('soundToggle').checked = sound;
        document.getElementById('autoArchive').checked = autoArchive;
        document.getElementById('sortBy').value = sortBy;

        this.currentSort = sortBy;
    }

    // Task Management
    addTask(text, priority = 'medium') {
        if (!text.trim()) {
            this.showToast('Please enter a task', 'warning');
            return;
        }

        const task = {
            id: Date.now(),
            text: text.trim(),
            priority,
            completed: false,
            createdAt: new Date().toISOString(),
            dueDate: null
        };

        this.tasks.unshift(task);
        this.saveTasks();
        this.render();
        this.showToast('Task added successfully', 'success');
        this.playSound();
    }

    deleteTask(id) {
        this.tasks = this.tasks.filter(task => task.id !== id);
        this.selectedTasks.delete(id);
        this.saveTasks();
        this.render();
        this.showToast('Task deleted', 'success');
    }

    toggleTask(id) {
        const task = this.tasks.find(t => t.id === id);
        if (task) {
            task.completed = !task.completed;
            this.saveTasks();
            this.render();
        }
    }

    editTask(id, newText) {
        const task = this.tasks.find(t => t.id === id);
        if (task) {
            task.text = newText.trim();
            this.saveTasks();
            this.render();
            this.showToast('Task updated', 'success');
        }
    }

    updatePriority(id, priority) {
        const task = this.tasks.find(t => t.id === id);
        if (task) {
            task.priority = priority;
            this.saveTasks();
            this.render();
        }
    }

    // Filtering & Searching
    filterTasks(filter) {
        this.currentFilter = filter;
        this.applyFiltersAndSort();
    }

    searchTasks(query) {
        const q = query.toLowerCase().trim();
        if (!q) {
            this.applyFiltersAndSort();
            return;
        }

        this.filteredTasks = this.tasks.filter(task => 
            task.text.toLowerCase().includes(q) ||
            task.priority.toLowerCase().includes(q)
        );
        this.renderTasks();
    }

    applyFiltersAndSort() {
        let filtered = [...this.tasks];

        // Apply filter
        switch (this.currentFilter) {
            case 'completed':
                filtered = filtered.filter(t => t.completed);
                break;
            case 'pending':
                filtered = filtered.filter(t => !t.completed);
                break;
            case 'high':
                filtered = filtered.filter(t => t.priority === 'high');
                break;
        }

        // Apply sort
        filtered = this.sortTasks(filtered);
        this.filteredTasks = filtered;
        this.renderTasks();
    }

    sortTasks(tasks) {
        const sorted = [...tasks];
        switch (this.currentSort) {
            case 'date-oldest':
                sorted.sort((a, b) => new Date(a.createdAt) - new Date(b.createdAt));
                break;
            case 'priority':
                const priorityOrder = { high: 3, medium: 2, low: 1 };
                sorted.sort((a, b) => priorityOrder[b.priority] - priorityOrder[a.priority]);
                break;
            case 'name':
                sorted.sort((a, b) => a.text.localeCompare(b.text));
                break;
            default: // date-newest
                sorted.sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt));
        }
        return sorted;
    }

    // Bulk Operations
    toggleTaskSelection(id) {
        if (this.selectedTasks.has(id)) {
            this.selectedTasks.delete(id);
        } else {
            this.selectedTasks.add(id);
        }
        this.updateBulkActions();
        this.renderTasks();
    }

    markSelectedComplete() {
        this.selectedTasks.forEach(id => {
            const task = this.tasks.find(t => t.id === id);
            if (task) task.completed = true;
        });
        this.selectedTasks.clear();
        this.saveTasks();
        this.render();
        this.showToast('Tasks marked as complete', 'success');
    }

    markSelectedIncomplete() {
        this.selectedTasks.forEach(id => {
            const task = this.tasks.find(t => t.id === id);
            if (task) task.completed = false;
        });
        this.selectedTasks.clear();
        this.saveTasks();
        this.render();
        this.showToast('Tasks marked as incomplete', 'success');
    }

    deleteSelected() {
        this.selectedTasks.forEach(id => this.deleteTask(id));
        this.selectedTasks.clear();
        this.render();
    }

    updateBulkActions() {
        const bulkActionsDiv = document.getElementById('bulkActions');
        const selectedCountSpan = document.getElementById('selectedCount');
        
        if (this.selectedTasks.size > 0) {
            bulkActionsDiv.style.display = 'flex';
            selectedCountSpan.textContent = `${this.selectedTasks.size} selected`;
        } else {
            bulkActionsDiv.style.display = 'none';
        }
    }

    // Statistics
    updateStats() {
        const total = this.tasks.length;
        const completed = this.tasks.filter(t => t.completed).length;
        const pending = total - completed;
        const percent = total === 0 ? 0 : Math.round((completed / total) * 100);

        document.getElementById('totalTasks').textContent = total;
        document.getElementById('completedTasks').textContent = completed;
        document.getElementById('pendingTasks').textContent = pending;
        document.getElementById('completionPercent').textContent = `${percent}%`;
    }

    // Rendering
    renderTasks() {
        const tasksList = document.getElementById('tasksList');
        const emptyState = document.getElementById('emptyState');

        if (this.filteredTasks.length === 0) {
            tasksList.innerHTML = '';
            emptyState.style.display = 'block';
            return;
        }

        emptyState.style.display = 'none';
        tasksList.innerHTML = this.filteredTasks.map(task => `
            <div class="task-item priority-${task.priority} ${task.completed ? 'completed' : ''} ${this.selectedTasks.has(task.id) ? 'selected' : ''}">
                <input 
                    type="checkbox" 
                    class="checkbox" 
                    ${task.completed ? 'checked' : ''}
                    onchange="taskManager.toggleTask(${task.id})"
                >
                <input 
                    type="checkbox" 
                    class="select-checkbox" 
                    style="display: none;"
                    ${this.selectedTasks.has(task.id) ? 'checked' : ''}
                    onchange="taskManager.toggleTaskSelection(${task.id})"
                >
                <div class="task-content">
                    <div class="task-text">${this.escapeHtml(task.text)}</div>
                    <div class="task-meta">
                        <span class="priority-badge ${task.priority}">${task.priority.toUpperCase()}</span>
                        <span class="task-date">📅 ${new Date(task.createdAt).toLocaleDateString()}</span>
                    </div>
                </div>
                <div class="task-actions">
                    <button class="btn-icon btn-edit" onclick="taskManager.promptEdit(${task.id})" title="Edit">✏️</button>
                    <button class="btn-icon btn-delete" onclick="taskManager.confirmDelete(${task.id})" title="Delete">🗑️</button>
                </div>
            </div>
        `).join('');
    }

    render() {
        this.applyFiltersAndSort();
        this.updateStats();
        this.updateBulkActions();
    }

    // UI Interactions
    promptEdit(id) {
        const task = this.tasks.find(t => t.id === id);
        if (!task) return;

        const newText = prompt('Edit task:', task.text);
        if (newText !== null && newText.trim()) {
            this.editTask(id, newText);
        }
    }

    confirmDelete(id) {
        const modal = document.getElementById('confirmModal');
        const message = document.getElementById('confirmMessage');
        message.textContent = 'Are you sure you want to delete this task?';
        
        const confirmOk = document.getElementById('confirmOk');
        const newConfirmOk = confirmOk.cloneNode(true);
        confirmOk.parentNode.replaceChild(newConfirmOk, confirmOk);
        
        newConfirmOk.onclick = () => {
            this.deleteTask(id);
            modal.classList.remove('active');
        };
        
        modal.classList.add('active');
    }

    // Export/Import
    exportTasks() {
        const dataStr = JSON.stringify(this.tasks, null, 2);
        const dataBlob = new Blob([dataStr], { type: 'application/json' });
        const url = URL.createObjectURL(dataBlob);
        const link = document.createElement('a');
        link.href = url;
        link.download = `tasks_${new Date().toISOString().split('T')[0]}.json`;
        link.click();
        this.showToast('Tasks exported successfully', 'success');
    }

    importTasks(file) {
        const reader = new FileReader();
        reader.onload = (e) => {
            try {
                const imported = JSON.parse(e.target.result);
                if (Array.isArray(imported)) {
                    this.tasks = [...this.tasks, ...imported];
                    this.saveTasks();
                    this.render();
                    this.showToast('Tasks imported successfully', 'success');
                } else {
                    this.showToast('Invalid file format', 'error');
                }
            } catch (err) {
                this.showToast('Error importing tasks', 'error');
            }
        };
        reader.readAsText(file);
    }

    // Utilities
    escapeHtml(text) {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    showToast(message, type = 'info') {
        const toast = document.getElementById('toast');
        toast.textContent = message;
        toast.className = `toast show ${type}`;
        
        setTimeout(() => {
            toast.classList.remove('show');
        }, 3000);
    }

    playSound() {
        if (this.getSetting('sound', true)) {
            const audioContext = new (window.AudioContext || window.webkitAudioContext)();
            const oscillator = audioContext.createOscillator();
            const gain = audioContext.createGain();
            
            oscillator.connect(gain);
            gain.connect(audioContext.destination);
            
            oscillator.frequency.value = 800;
            oscillator.type = 'sine';
            gain.gain.setValueAtTime(0.1, audioContext.currentTime);
            gain.gain.exponentialRampToValueAtTime(0.01, audioContext.currentTime + 0.1);
            
            oscillator.start(audioContext.currentTime);
            oscillator.stop(audioContext.currentTime + 0.1);
        }
    }

    registerServiceWorker() {
        if ('serviceWorker' in navigator) {
            navigator.serviceWorker.register('sw.js').catch(err => 
                console.log('Service Worker registration failed:', err)
            );
        }
    }

    // Dark Mode
    toggleDarkMode() {
        document.body.classList.toggle('dark-mode');
        const isDark = document.body.classList.contains('dark-mode');
        document.getElementById('darkModeBtn').textContent = isDark ? '☀️' : '🌙';
        this.saveSetting('darkMode', isDark);
    }

    clearAllTasks() {
        if (confirm('Are you sure you want to delete ALL tasks? This cannot be undone.')) {
            this.tasks = [];
            this.selectedTasks.clear();
            this.saveTasks();
            this.render();
            this.showToast('All tasks cleared', 'success');
        }
    }
}

// Initialize
const taskManager = new TaskManager();

// Event Listeners
document.addEventListener('DOMContentLoaded', () => {
    // Add Task
    document.getElementById('addBtn').addEventListener('click', () => {
        const input = document.getElementById('taskInput');
        const priority = document.getElementById('prioritySelect').value;
        taskManager.addTask(input.value, priority);
        input.value = '';
        input.focus();
    });

    document.getElementById('taskInput').addEventListener('keypress', (e) => {
        if (e.key === 'Enter') {
            document.getElementById('addBtn').click();
        }
    });

    // Filter
    document.querySelectorAll('.filter-btn').forEach(btn => {
        btn.addEventListener('click', (e) => {
            document.querySelectorAll('.filter-btn').forEach(b => b.classList.remove('active'));
            e.target.classList.add('active');
            taskManager.filterTasks(e.target.dataset.filter);
        });
    });

    // Search
    document.getElementById('searchInput').addEventListener('input', (e) => {
        taskManager.searchTasks(e.target.value);
    });

    // Dark Mode
    document.getElementById('darkModeBtn').addEventListener('click', () => {
        taskManager.toggleDarkMode();
    });

    // Settings Modal
    document.getElementById('settingsBtn').addEventListener('click', () => {
        document.getElementById('settingsModal').classList.add('active');
    });

    document.querySelector('.close-btn').addEventListener('click', () => {
        document.getElementById('settingsModal').classList.remove('active');
    });

    document.getElementById('closeSettingsBtn').addEventListener('click', () => {
        document.getElementById('settingsModal').classList.remove('active');
    });

    // Settings Changes
    document.getElementById('notificationsToggle').addEventListener('change', (e) => {
        taskManager.saveSetting('notifications', e.target.checked);
    });

    document.getElementById('soundToggle').addEventListener('change', (e) => {
        taskManager.saveSetting('sound', e.target.checked);
    });

    document.getElementById('autoArchive').addEventListener('change', (e) => {
        taskManager.saveSetting('autoArchive', e.target.checked);
    });

    document.getElementById('sortBy').addEventListener('change', (e) => {
        taskManager.currentSort = e.target.value;
        taskManager.saveSetting('sortBy', e.target.value);
        taskManager.render();
    });

    // Bulk Actions
    document.getElementById('markCompleteBtn').addEventListener('click', () => {
        taskManager.markSelectedComplete();
    });

    document.getElementById('markIncompleteBtn').addEventListener('click', () => {
        taskManager.markSelectedIncomplete();
    });

    document.getElementById('deleteBulkBtn').addEventListener('click', () => {
        if (confirm('Delete selected tasks?')) {
            taskManager.deleteSelected();
        }
    });

    // Footer Actions
    document.getElementById('exportBtn').addEventListener('click', (e) => {
        e.preventDefault();
        taskManager.exportTasks();
    });

    document.getElementById('importBtn').addEventListener('click', (e) => {
        e.preventDefault();
        const input = document.createElement('input');
        input.type = 'file';
        input.accept = '.json';
        input.addEventListener('change', (event) => {
            if (event.target.files[0]) {
                taskManager.importTasks(event.target.files[0]);
            }
        });
        input.click();
    });

    document.getElementById('clearAllBtn').addEventListener('click', (e) => {
        e.preventDefault();
        taskManager.clearAllTasks();
    });

    // Confirm Modal
    document.getElementById('confirmCancel').addEventListener('click', () => {
        document.getElementById('confirmModal').classList.remove('active');
    });

    // Close modals on outside click
    document.querySelectorAll('.modal').forEach(modal => {
        modal.addEventListener('click', (e) => {
            if (e.target === modal) {
                modal.classList.remove('active');
            }
        });
    });
});
