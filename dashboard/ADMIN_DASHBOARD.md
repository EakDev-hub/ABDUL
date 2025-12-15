# Admin Dashboard Guide

## Overview

The Admin Dashboard provides a professional interface to manage Q&A and Announcements for the ABDUL Hackathon system.

## Login Credentials

To access the admin dashboard, use the following credentials:

- **Username:** `nury`
- **Password:** `pastelsoftcream`

The authentication uses simple localStorage-based session management. Once logged in, you'll remain authenticated until you click the logout button.

## Features

### Q&A Management
- ✅ View all Q&A records in a table format
- ➕ Create new Q&A entries
- ✏️ Edit existing Q&A
- 🗑️ Delete Q&A records
- 🔄 Auto-refresh every 10 seconds

### Announcement Management
- ✅ View all announcements
- ➕ Create new announcements
- ✏️ Edit existing announcements
- 🗑️ Delete announcements
- 📅 Set custom posted date/time
- 🔘 Toggle active/inactive status
- 🔄 Auto-refresh every 10 seconds

## Access

### Development
- Main Dashboard: `http://localhost:5173/`
- Admin Dashboard: `http://localhost:5173/admin.html`

### Production
- Main Dashboard: `https://your-domain.com/`
- Admin Dashboard: `https://your-domain.com/admin.html`

## API Endpoints

All admin endpoints are located at `/api/admin/*`

### Q&A Endpoints
- `GET /api/admin/qna` - Get all Q&A records
- `GET /api/admin/qna/{id}` - Get specific Q&A
- `POST /api/admin/qna` - Create new Q&A
- `PUT /api/admin/qna/{id}` - Update Q&A
- `DELETE /api/admin/qna/{id}` - Delete Q&A

### Announcement Endpoints
- `GET /api/admin/announcements` - Get all announcements
- `GET /api/admin/announcements/{id}` - Get specific announcement
- `POST /api/admin/announcements` - Create new announcement
- `PUT /api/admin/announcements/{id}` - Update announcement
- `DELETE /api/admin/announcements/{id}` - Delete announcement

## Usage Guide

### Creating Q&A
1. Click "➕ Add New Q&A" button
2. Enter the question (required)
3. Enter the answer (optional)
4. Click "Save"

### Editing Q&A
1. Click "✏️ Edit" button on any row
2. Modify the question or answer
3. Click "Save"

### Deleting Q&A
1. Click "🗑️ Delete" button on any row
2. Confirm deletion in the popup
3. Record will be removed immediately

### Creating Announcements
1. Click "➕ Add New Announcement" button
2. Enter the announcement text (required)
3. Set posted date/time (defaults to now)
4. Toggle active/inactive status
5. Click "Save"

### Editing Announcements
1. Click "✏️ Edit" button on any row
2. Modify text, date, or status
3. Click "Save"

### Deleting Announcements
1. Click "🗑️ Delete" button on any row
2. Confirm deletion in the popup
3. Record will be removed immediately

## Design Features

- **Professional gradient theme** with purple/violet colors
- **Responsive design** - works on desktop and mobile
- **Real-time updates** - auto-refreshes data every 10 seconds
- **Modern UI components** - modals, tables, buttons with smooth animations
- **User-friendly** - clear labels, intuitive actions, confirmation dialogs

## Technical Details

### Frontend Stack
- Vue 3 with TypeScript
- Composition API
- Reactive state management
- Auto-refresh functionality

### Backend Stack
- ASP.NET Core Web API
- PostgreSQL database
- RESTful API design
- CRUD operations

## Security Note

⚠️ **Important**: This admin dashboard currently has no authentication. In production, you should:
1. Add authentication middleware
2. Implement role-based access control
3. Use HTTPS only
4. Add CORS restrictions
5. Implement rate limiting

## Troubleshooting

### Admin page not loading
- Check if Vite dev server is running
- Verify the admin.html file exists
- Check browser console for errors

### API calls failing
- Verify backend server is running
- Check API_GATEWAY_URL in .env file
- Review CORS settings
- Check network tab in browser dev tools

### Changes not appearing
- Wait for auto-refresh (10 seconds)
- Manually refresh the page
- Check if API endpoints are responding

## Development

### Running Locally
```bash
cd dashboard
npm install
npm run dev
```

### Building for Production
```bash
npm run build
```

This will create both `index.html` and `admin.html` in the `dist` folder.

## Support

For issues or questions, please check the main README.md or contact the development team.