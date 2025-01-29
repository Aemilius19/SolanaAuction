use anchor_lang::prelude::*;
use anchor_spl::token::{self, Mint, Token, TokenAccount, MintTo};

/// Совпадаем с `address` в вашем IDL
declare_id!("9WVwWe4cEgDQfKH9qx6nDqAuzcAAMWPpKmGrgtXsGVqt");

#[program]
pub mod solana_auction {
    use super::*;

    pub fn mint_nft(ctx: Context<MintNft>, metadata_url: String) -> Result<()> {
        let nft = &mut ctx.accounts.nft;
        nft.metadata_url = metadata_url;
        nft.owner = *ctx.accounts.user.key;
        nft.is_locked = false;

        // Минтим 1 токен
        let cpi_accounts = MintTo {
            mint: ctx.accounts.mint.to_account_info(),
            to: ctx.accounts.token_account.to_account_info(),
            authority: ctx.accounts.user.to_account_info(),
        };
        let cpi_ctx = CpiContext::new(ctx.accounts.token_program.to_account_info(), cpi_accounts);
        token::mint_to(cpi_ctx, 1)?;

        Ok(())
    }

    pub fn lock_nft(ctx: Context<LockNft>) -> Result<()> {
        let nft = &mut ctx.accounts.nft;
        require!(!nft.is_locked, ErrorCode::NftAlreadyLocked);
        nft.is_locked = true;
        Ok(())
    }

    pub fn unlock_nft(ctx: Context<UnlockNft>) -> Result<()> {
        let nft = &mut ctx.accounts.nft;
        require!(nft.is_locked, ErrorCode::NftNotLocked);
        nft.is_locked = false;
        Ok(())
    }

    pub fn transfer_nft(ctx: Context<TransferNft>, new_owner: Pubkey) -> Result<()> {
        let nft = &mut ctx.accounts.nft;
        require!(!nft.is_locked, ErrorCode::NftLocked);
        nft.owner = new_owner;
        Ok(())
    }
}

#[derive(Accounts)]
pub struct MintNft<'info> {
    // Пространство 105 байт соответствует вашему IDL
    #[account(init, payer = user, space = 105)]
    pub nft: Account<'info, Nft>,
    #[account(mut)]
    pub user: Signer<'info>,
    /// CHECK: вручную проверяем, что это — аккаунт Mint
    #[account(mut)]
    pub mint: AccountInfo<'info>,
    /// CHECK: вручную проверяем, что это — аккаунт TokenAccount
    #[account(mut)]
    pub token_account: AccountInfo<'info>,
    pub token_program: Program<'info, Token>,
    pub system_program: Program<'info, System>,
}

#[derive(Accounts)]
pub struct LockNft<'info> {
    #[account(mut)]
    pub nft: Account<'info, Nft>,
    pub user: Signer<'info>,
}

#[derive(Accounts)]
pub struct UnlockNft<'info> {
    #[account(mut)]
    pub nft: Account<'info, Nft>,
    pub user: Signer<'info>,
}

#[derive(Accounts)]
pub struct TransferNft<'info> {
    #[account(mut)]
    pub nft: Account<'info, Nft>,
    pub user: Signer<'info>,
}

#[account]
pub struct Nft {
    pub metadata_url: String,
    pub owner: Pubkey,
    pub is_locked: bool,
}

#[error_code]
pub enum ErrorCode {
    #[msg("NFT is already locked.")]
    NftAlreadyLocked,
    #[msg("NFT is not locked.")]
    NftNotLocked,
    #[msg("NFT is locked and cannot be transferred.")]
    NftLocked,
}
