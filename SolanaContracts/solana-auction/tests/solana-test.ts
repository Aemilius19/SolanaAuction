import * as anchor from '@project-serum/anchor';
import { Program } from '@project-serum/anchor';
import { SolanaAuction } from '../target/types/solana_auction';

describe('solana-auction', () => {
  const provider = anchor.AnchorProvider.env();
  anchor.setProvider(provider);

  const program = anchor.workspace.SolanaAuction as Program<SolanaAuction>;

  it('Mints an NFT', async () => {
    const metadataUrl = "https://example.com/nft.json";
    const nftKeypair = anchor.web3.Keypair.generate();

    await program.methods.mintNft(metadataUrl)
      .accounts({
        nft: nftKeypair.publicKey,
        user: provider.wallet.publicKey,
        mint: /* ваш mint аккаунт */,
        tokenAccount: /* ваш token аккаунт */,
        tokenProgram: anchor.utils.token.TOKEN_PROGRAM_ID,
        systemProgram: anchor.web3.SystemProgram.programId,
      })
      .signers([nftKeypair])
      .rpc();

    const nftAccount = await program.account.nft.fetch(nftKeypair.publicKey);
    console.log('NFT created:', nftAccount);
  });

  it('Locks an NFT', async () => {
    const nftKeypair = anchor.web3.Keypair.generate();

    await program.methods.lockNft()
      .accounts({
        nft: nftKeypair.publicKey,
        user: provider.wallet.publicKey,
      })
      .signers([nftKeypair])
      .rpc();

    const nftAccount = await program.account.nft.fetch(nftKeypair.publicKey);
    console.log('NFT locked:', nftAccount);
  });

  it('Unlocks an NFT', async () => {
    const nftKeypair = anchor.web3.Keypair.generate();

    await program.methods.unlockNft()
      .accounts({
        nft: nftKeypair.publicKey,
        user: provider.wallet.publicKey,
      })
      .signers([nftKeypair])
      .rpc();

    const nftAccount = await program.account.nft.fetch(nftKeypair.publicKey);
    console.log('NFT unlocked:', nftAccount);
  });

  it('Transfers an NFT', async () => {
    const nftKeypair = anchor.web3.Keypair.generate();
    const newOwner = anchor.web3.Keypair.generate().publicKey;

    await program.methods.transferNft(newOwner)
      .accounts({
        nft: nftKeypair.publicKey,
        user: provider.wallet.publicKey,
      })
      .signers([nftKeypair])
      .rpc();

    const nftAccount = await program.account.nft.fetch(nftKeypair.publicKey);
    console.log('NFT transferred:', nftAccount);
  });
});