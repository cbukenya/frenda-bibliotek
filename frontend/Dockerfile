# ─── Deps stage ──────────────────────────────────────────────────────────────
FROM node:20-alpine AS deps
WORKDIR /app
COPY package.json ./
RUN npm install

# ─── Build stage ─────────────────────────────────────────────────────────────
FROM node:20-alpine AS build
WORKDIR /app
# Bake the internal API URL into the Next.js build (rewrites are resolved at build time)
ENV NEXT_PUBLIC_API_URL=http://api:5000
COPY --from=deps /app/node_modules ./node_modules
COPY . .
RUN npm run build

# ─── Runtime stage ───────────────────────────────────────────────────────────
FROM node:20-alpine AS runtime
WORKDIR /app
ENV NODE_ENV=production
COPY --from=build /app/public ./public
COPY --from=build /app/.next/standalone ./
COPY --from=build /app/.next/static ./.next/static
EXPOSE 3000
CMD ["node", "server.js"]
