#!/bin/bash
# Download plugin packages
# - NuGet packages (Essentials + Biamp) from nuget.org
# - Cisco v2.6.0 from GitHub releases (not available on NuGet)
echo "Downloading packages..."
echo ""

# Install NuGet packages (Essentials + Biamp)
echo "Installing packages from NuGet..."
nuget install ./packages.config -OutputDirectory ./packages

# Download Cisco v2.6.0 from GitHub (not available on NuGet)
echo ""
echo "Downloading Cisco codec v2.6.0 from GitHub..."
mkdir -p "packages/epi-videoCodec-ciscoExtended"
curl -L -o "packages/epi-videoCodec-ciscoExtended/epi-videoCodec-ciscoExtended.4Series.2.6.0.cplz" \
  https://github.com/PepperDash/epi-videoCodec-ciscoExtended/releases/download/v2.6.0/epi-videoCodec-ciscoExtended.4Series.2.6.0.cplz

# Download Crestron IO v2.2.0 from GitHub (not available on NuGet)
echo ""
echo "Downloading Crestron IO v2.2.0 from GitHub..."
mkdir -p "packages/epi-crestron-io"
curl -L -o "packages/epi-crestron-io/epi-crestron-io.4Series.2.2.0.net472.cplz" \
  https://github.com/PepperDash/epi-crestron-io/releases/download/v2.2.0/epi-crestron-io.4Series.2.2.0.net472.cplz

echo ""
echo "========================================"
echo "✅ Package download complete!"
echo "========================================"
echo ""
