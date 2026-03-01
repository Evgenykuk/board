-- sql_scheme/board_db.sql
create table planes (
    plane_id text primary key,
    flight_id text,
    capacity int not null check (capacity >= 0),
    fuel_level int not null default 0 check (fuel_level >= 0),
    fuel_required int not null default 0 check (fuel_required >= 0),
    state text not null,
    location_node text not null,
    route_id uuid,
    created_at timestamp not null default now(),
    updated_at timestamp not null default now()
);

create index planes_flight_idx on planes(flight_id);
create index planes_state_idx on planes(state);

create table plane_checklist (
    plane_id text primary key,
    need_fuel boolean not null default false,
    need_catering boolean not null default true,
    need_bus boolean not null default true,
    need_followme boolean not null default true,
    fuel_done boolean not null default false,
    catering_done boolean not null default false,
    bus_done boolean not null default false,
    followme_done boolean not null default false,
    passengers_expected int not null default 0,
    passengers_boarded int not null default 0,
    updated_at timestamp not null default now()
);

create table plane_manifest (
    plane_id text not null,
    passenger_id text not null,
    boarded_at timestamp not null default now(),
    primary key (plane_id, passenger_id)
);

create table plane_state_history (
    id bigserial primary key,
    plane_id text not null,
    old_state text,
    new_state text not null,
    sim_time timestamp,
    created_at timestamp not null default now()
);

create table processed_events (
    event_id uuid primary key,
    processed_at timestamp not null default now()
);
