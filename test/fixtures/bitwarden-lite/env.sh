# Deterministic env values exercising as many template branches as possible.
# Mix of overrides, defaults left unset, enables/disables, and a Split/Trim list.
export BW_DOMAIN=test.example.com
export BW_ENABLE_SSL=true
export BW_ENABLE_IPV6=true
export BW_ENABLE_SSL_DH=true
export BW_ENABLE_SSL_CA=true
export BW_REAL_IPS="10.0.0.1, 10.0.0.2 ,10.0.0.3"
export BW_ENABLE_API=true
export BW_ENABLE_ICONS=true
export BW_ICONS_PROXY_TO_CLOUD=false
export BW_ENABLE_NOTIFICATIONS=true
export BW_ENABLE_EVENTS=true
export BW_ENABLE_SSO=true
export BW_ENABLE_IDENTITY=true
export BW_ENABLE_ADMIN=true
export BW_ENABLE_SCIM=true
export BW_ENABLE_KEY_CONNECTOR=true
export BW_KEY_CONNECTOR_INTERNAL_URL=http://keyconnector:5000
export globalSettings__baseServiceUri__vault=https://test.example.com
