# Clearshore
 
Independently branded, multilingual web platform for DESMI Seaweed, with an admin dashboard and approval workflow so a non-technical team can keep content up to date.
 
## About
 
DESMI Seaweed is the business unit of the Danish marine equipment manufacturer DESMI that develops floating mesh booms and removal units against sargassum on Caribbean coastlines. Its solutions are deployed across twelve Caribbean countries, yet the unit exists online only as a subsection of DESMI's much larger corporate website. Its content is not maintained on a regular schedule, and the people responsible have no structured way to publish updates without bypassing oversight or relying on outside technical help.
 
This project gives DESMI Seaweed its own branded online presence and a controlled way to maintain it.
 
## Features
 
**Public website for visitors** (hotels, marinas, national maritime authorities, international NGOs)
 
- Available in English, Spanish and French
- Interactive map of installations shown at country or city level, never exact coordinates
- News and updates, product information and contact
**Admin dashboard for employees**
 
- Create and edit website content without outside technical help
- Approval workflow: draft, pending, approved, live
- Nothing is published without review
## Architecture
 
The system is a three-tier application.
 
| Tier | Component | Technology |
|---|---|---|
| 1 | Public website for clients | React |
| 1 | Admin dashboard for employees | Blazor |
| 2 | Business logic server exposing a REST API | .NET |
| 3 | Data server and database | PostgreSQL |
 
Both clients communicate with the business logic server over REST. The business logic server is the only component that talks to the data server.
 
 
## Project scope
 
- The site is hosted on its own domain, separate from DESMI's existing website
- Branding is independent, leaving room for a future name change if DESMI Seaweed separates from DESMI as a business unit
- Multilingual support is part of the application and does not rely on an outside translation service
- Geographic data is limited to country or city level
## Process
 
The project follows the Unified Process (Inception, Elaboration, Construction, Transition) and is managed with Scrumban: fixed-length sprints with planning, standups and retrospectives, combined with work-in-progress limits and a visual board.
 
## Team
 
- Adrian Mikkel Escartin Lam
- Alexandru Potfalean
- Marina Montserrat Chavez Egas
- Marko Stokic
 
Developed as the 3rd semester project (SEP3) in Software Technology Engineering at VIA University College, Horsens.
