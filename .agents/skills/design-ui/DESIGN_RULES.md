---
name: airbnb-design-system
description: Airbnb-inspired modern UI system focused on clean layouts, accessibility, responsive design, subtle interactions, and production-ready frontend patterns.
---

# Airbnb Design System

## Trigger

Use this skill when user asks about:

- UI design
- frontend interfaces
- React UI
- Tailwind CSS
- responsive layouts
- forms
- landing pages
- dashboards
- SaaS UI
- design systems
- accessibility
- component libraries
- modern web design

---

# Core Design Principles

- Clean and calm interfaces
- Generous whitespace
- Minimal visual noise
- Rounded and approachable UI
- Human-centered interactions
- Mobile-first responsive design
- High readability
- Subtle hierarchy
- Consistent spacing and typography
- Soft elevation instead of hard borders

---

# Visual Heuristics

- Prefer breathing room over dense layouts
- One primary focal point per section
- Reduce cognitive load aggressively
- Use color intentionally, not excessively
- Prioritize clarity before decoration
- Use contrast sparingly
- Keep visual rhythm consistent
- Interfaces should feel lightweight and welcoming

---

# Color System

## Primary

- Rausch Pink: #FF5A5F
- Rausch Pink Hover: #E31C5F

## Text Colors

- Primary Text: #222222
- Secondary Text: #717171
- Muted Text: #B0B0B0

## Borders & Dividers

- Primary Border: #DDDDDD
- Divider Border: #EBEBEB

## Backgrounds

- White: #FFFFFF
- Soft Gray: #F7F7F7

## Semantic Colors

### Success

- #008A05

### Warning

- #FFB400

### Error

- #D93025

### Info

- #006AFF

---

# Typography

## Font Family

Use:

- Circular
- Inter
- system-ui fallback

## Font Weights

- Regular: 400
- Medium: 500
- Semibold: 600

Avoid excessive bold text.

## Font Sizes

### Headings

- H1: 32px
- H2: 24px
- H3: 20px

### Body

- Body Large: 16px
- Body Small: 14px
- Caption: 12px

## Line Height

- Headings: 1.2
- Body: 1.5
- Long content: 1.6

## Letter Spacing

- Headings: -0.5px
- Body: 0px
- Captions: 0.3px

---

# Spacing System

Use strict 8px grid spacing.

## Allowed Spacing Values

- 4px
- 8px
- 12px
- 16px
- 24px
- 32px
- 48px
- 64px

Avoid arbitrary spacing values.

---

# Layout Rules

## Section Spacing

- Mobile section gap: 32px
- Desktop section gap: 48px

## Container Padding

- Mobile: 16px
- Tablet/Desktop: 24px

## Content Width

- Avoid extremely wide text blocks
- Preferred readable width: 640-720px

## Layout Strategy

Prefer:

- flexbox
- CSS grid when necessary

Avoid:

- deeply nested layouts
- overcrowded sections

---

# Component Sizing

## Buttons

- Height: 48px
- Radius: 12px
- Horizontal Padding: 24px

## Inputs

- Height: 48px
- Radius: 12px
- Horizontal Padding: 12px
- Vertical Padding: 14px

## Cards

- Radius: 12px
- Internal Padding: 24px

## Checkbox & Radio

- Size: 16x16px

## Avatars

- Small: 32x32px
- Medium: 48x48px
- Large: 80x80px

## Touch Targets

Minimum:

- 48x48px

---

# Borders & Shadows

## Borders

Use subtle borders only.

Preferred:

- 1px solid #EBEBEB

Focus/Error:

- 2px solid #FF5A5F

Avoid:

- thick borders
- dark borders
- decorative outlines

## Shadows

### Subtle

```css
box-shadow: 0 2px 8px rgba(0,0,0,0.08);
````

### Elevated

```css
box-shadow: 0 4px 12px rgba(0,0,0,0.12);
```

Avoid:

- heavy shadows
- colored shadows
- dramatic glow effects

---

# Responsive Design

## Mobile First

Always design mobile-first.

## Breakpoints

- sm: 640px
- md: 768px
- lg: 1024px
- xl: 1280px

## Responsive Rules

- Stack vertically before shrinking content
- Preserve readability on small screens
- Avoid horizontal scrolling
- Keep touch targets accessible

---

# Interactive States

## Hover

- subtle background shift
- slight elevation
- soft transition

## Active

- slight press effect
- optional scale: 0.98

## Focus

- 2px solid #FF5A5F
- visible outline
- accessible contrast
- keyboard navigable

## Disabled

- reduced opacity
- remove hover effects
- cursor: not-allowed

---

# Animation Guidelines

## Duration

Preferred:

- 200ms

Maximum:

- 250ms

## Easing

Use:

- ease-out

## Animation Style

Prefer:

- subtle motion
- lightweight transitions
- gentle fades

Avoid:

- bouncing animations
- exaggerated movement
- long transitions
- distracting effects

---

# Form Design Patterns

## Inputs

- Always include labels
- Use helper text when necessary
- Use placeholder as support only
- Maintain consistent spacing

## Validation

- Show validation clearly
- Place error messages below fields
- Use semantic colors consistently

## CTA Buttons

- Only one primary CTA per section
- Primary CTA should stand out clearly

---

# Accessibility

Follow WCAG 2.1 AA standards.

## Accessibility Rules

- Minimum contrast ratio AA
- Keyboard navigation support
- Proper aria-label usage
- Semantic HTML
- Visible focus states
- Screen reader friendly
- Accessible form labels
- Accessible error messages

## Images

- Always include meaningful alt text

## Touch Accessibility

Minimum target:

- 48x48px

---

# Code Patterns

## Button Example

Use:

- rounded-xl
- h-12
- font-medium
- transition-all
- focus-visible:outline-none
- focus-visible:ring-2

## Card Example

Use:

- rounded-xl
- bg-white
- border border-gray-200
- shadow-sm

## Layout Example

Prefer:

- flex
- gap-4
- max-w-screen-lg
- mx-auto

---

# Decision Rules

## Layout Decisions

If layout feels crowded:

- increase whitespace before reducing font size

If multiple CTAs exist:

- highlight only one primary action

If mobile layout breaks:

- stack vertically before shrinking elements

If hierarchy feels weak:

- increase spacing before increasing colors

---

# Detect And Avoid

- nested cards
- excessive gradients
- multiple accent colors
- centered long paragraphs
- excessive icons
- cluttered layouts
- oversized modals
- sharp corners
- inconsistent spacing
- decorative animations
- dense information walls

---

# Output Constraints

- Always generate responsive layouts
- Always include accessibility considerations
- Prefer Tailwind utility classes
- Avoid arbitrary spacing values
- Avoid unfinished JSX
- Prefer reusable components
- Maintain spacing consistency
- Use semantic HTML structure

---

# Self Review Checklist

Before finalizing output:

1. Check spacing consistency
2. Check typography hierarchy
3. Check responsive behavior
4. Check accessibility compliance
5. Check focus states
6. Check hover states
7. Check mobile usability
8. Check visual hierarchy
9. Remove unnecessary complexity
10. Validate component consistency
11. Ensure CTA clarity
12. Ensure clean alignment

---

# Constraints

- Avoid visual clutter
- Avoid dark heavy UI
- Avoid dense layouts
- Avoid inconsistent spacing
- Avoid unnecessary animations
- Avoid multiple competing accents
- Avoid inaccessible contrast
- Avoid giant components
- Avoid over-engineering simple layouts

---

# Final Goal

Interfaces should feel:

- calm
- premium
- approachable
- modern
- effortless
- highly usable
- visually lightweight
- production ready
